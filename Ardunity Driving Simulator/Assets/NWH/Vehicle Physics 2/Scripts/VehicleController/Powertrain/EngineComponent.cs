using System;
using System.Collections.Generic;
using NWH.Common.Utility;
using UnityEngine;
using UnityEngine.Events;
using NWH.Common.Vehicles;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
#endif

namespace NWH.VehiclePhysics2.Powertrain
{
    [Serializable]
    public partial class EngineComponent : PowertrainComponent
    {
        public delegate float CalculateTorque(float angularVelocity, float dt);

        /// <summary>
        ///     Delegate for a function that modifies engine power.
        /// </summary>
        public delegate float PowerModifier();


        public enum EngineType
        {
            ICE,
            Electric,
        }


        /// <summary>
        ///     If true starter will be ran for [starterRunTime] seconds if engine receives any throttle input.
        /// </summary>
        [Tooltip("    If true starter will be ran for [starterRunTime] seconds if engine receives any throttle input.")]
        public bool autoStartOnThrottle = true;


        /// <summary>
        ///     Assign your own delegate to use different type of torque calculation.
        /// </summary>
        [Tooltip("    Assign your own delegate to use different type of torque calculation.")]
        public CalculateTorque calculateTorqueDelegate;


        /// <summary>
        ///     Engine type. ICE (Internal Combustion Engine) supports features such as starter, stalling, etc.
        ///     Electric engine (motor) can run in reverse, can not be stalled and does not use starter.
        /// </summary>
        [Tooltip(
            "Engine type. ICE (Internal Combustion Engine) supports features such as starter, stalling, etc.\r\nElectric engine (motor) can run in reverse, can not be stalled and does not use starter.")]
        [ShowInTelemetry]
        [ShowInSettings]
        public EngineType engineType = EngineType.ICE;


        /// <summary>
        ///     Turbocharger or supercharger.
        /// </summary>
        [Tooltip("    Turbocharger or supercharger.")]
        public ForcedInduction forcedInduction = new ForcedInduction();


        /// <summary>
        ///     Power generated by the engine in kW
        /// </summary>
        [Tooltip("    Power generated by the engine in kW")]
        [ShowInTelemetry]
        [NonSerialized]
        public float generatedPower;


        /// <summary>
        ///     RPM at which idler circuit will try to keep RPMs when there is no input.
        /// </summary>
        [SerializeField]
        [Tooltip("    RPM at which idler circuit will try to keep RPMs when there is no input.")]
        public float idleRPM = 900;



        /// <summary>
        ///     Maximum engine power in [kW].
        /// </summary>
        [Tooltip("    Maximum engine power in [kW].")]
        [ShowInSettings("Max. Power", 20, 400, 10)]
        public float maxPower = 120;
        
        
        /// <summary>
        /// Loss power (pumping, friction losses) is calculated as the percentage of maxPower.
        /// Should be between 0 and 1 (100%).
        /// </summary>
        [Range(0, 1)]
        public float engineLossPercent = 0.25f;

        
        /// <summary>
        ///     Called when engine hits rev limiter.
        /// </summary>
        [Tooltip("    Called when engine hits rev limiter.")]
        public UnityEvent onRevLimiter = new UnityEvent();


        /// <summary>
        ///     Called when engine is started.
        ///     Param is true if using starter.
        /// </summary>
        [Tooltip("    Called when engine is started.")]
        public UnityEvent onStart = new UnityEvent();


        /// <summary>
        ///     Called when engine is stopped.
        ///     Param is true if stopping by request, instead of stalling.
        /// </summary>
        [Tooltip("    Called when engine is stopped.")]
        public UnityEvent onStop = new UnityEvent();


        /// <summary>
        ///     If true the engine will be started immediately, without running the starter, when the vehicle is enabled.
        ///     Sets engine angular velocity to idle angular velocity.
        /// </summary>
        [Tooltip(
            "If true the engine will be started immediately, without running the starter, when the vehicle is enabled.\r\nSets engine angular velocity to idle angular velocity.")]
        public bool flyingStartEnabled;


        public bool ignition = true;


        /// <summary>
        ///     Power curve with RPM range [0,1] on the X axis and power coefficient [0,1] on Y axis.
        ///     Both values are represented as percentages and should be in 0 to 1 range.
        ///     Power coefficient is multiplied by maxPower to get the final power at given RPM.
        /// </summary>
        [Tooltip(
            "Power curve with RPM range [0,1] on the X axis and power coefficient [0,1] on Y axis.\r\nBoth values are represented as percentages and should be in 0 to 1 range.\r\nPower coefficient is multiplied by maxPower to get the final power at given RPM.")]
        public AnimationCurve powerCurve;


        /// <summary>
        ///     List of callbacks that influence engine power. Examples would be traction control which
        ///     reduces power (returns less than 1) or forced induction which increases power (returns more than 1).
        ///     Can also be used by modules to reduce engine power in certain situations.
        ///     Final power modifier value is calculated by multiplying return values of all callbacks.
        /// </summary>
        [Tooltip(
            "List of callbacks that influence engine power. Examples would be traction control which\r\nreduces power (returns less than 1) or forced induction which increases power (returns more than 1).\r\nCan also be used by modules to reduce engine power in certain situations.\r\nFinal power modifier value is calculated by multiplying return values of all callbacks.")]
        [NonSerialized] public List<PowerModifier> powerModifiers = new List<PowerModifier>();


        /// <summary>
        ///     Is the engine currently hitting the rev limiter?
        /// </summary>
        [Tooltip("    Is the engine currently hitting the rev limiter?")]
        [NonSerialized] public bool revLimiterActive;


        /// <summary>
        ///     If engine RPM rises above revLimiterRPM, how long should fuel cutoff last?
        ///     Higher values make hitting rev limiter more rough and choppy.
        /// </summary>
        [Tooltip(
            "If engine RPM rises above revLimiterRPM, how long should fuel cutoff last?\r\nHigher values make hitting rev limiter more rough and choppy.")]
        public float revLimiterCutoffDuration = 0f;


        /// <summary>
        ///     Engine RPM at which rev limiter activates.
        /// </summary>
        [Tooltip("    Engine RPM at which rev limiter activates.")]
        public float revLimiterRPM = 4700;


        /// <summary>
        ///     Can the vehicle be stalled?
        ///     If disabled engine will run no matter the RPM. Automatically disabled when electric engine type is used.
        /// </summary>
        [Tooltip(
            "Can the vehicle be stalled?\r\nIf disabled engine will run no matter the RPM. Automatically disabled when electric engine type is used.")]
        public bool stallingEnabled = true;


        /// <summary>
        ///     Is the starter currently active?
        /// </summary>
        [Tooltip("    Is the starter currently active?")]
        [NonSerialized]
        [ShowInTelemetry]
        public bool starterActive = false;

        /// <summary>
        ///     Torque starter motor can put out. Make sure that this torque is more than loss torque
        ///     at the starter RPM limit. If too low the engine will fail to start.
        /// </summary>
        [Tooltip(
            "Torque starter motor can put out. Make sure that this torque is more than loss torque\r\nat the starter RPM limit. If too low the engine will fail to start.")]
        public float startDuration = 0.5f;
        
        private float _starterTorque;
        private float _idleAngularVelocity;
        private float _stallAngularVelocity;
        private float _revLimiterAngularVelocity;
        private float _powerModifierSum;
        private Coroutine _starterCoroutine;
        private float _userThrottleInput;
        private float _prevAngularVelocity;
        private float _prevDamage;


        /// <summary>
        /// Peak power as calculated from the power curve. If the power curve peaks at Y=1 peak power will equal max power field value.
        /// After changing power, power curve or RPM range call UpdatePeakPowerAndTorque() to get update the value.
        /// </summary>
        public float EstimatedPeakPower
        {
            get { return _peakPower; }
        }
        private float _peakPower;


        /// <summary>
        /// RPM at which the peak power is achieved.
        /// After changing power, power curve or RPM range call UpdatePeakPowerAndTorque() to get update the value.
        /// </summary>
        public float EstimatedPeakPowerRPM
        {
            get { return _peakPowerRpm; }
        }
        private float _peakPowerRpm;


        /// <summary>
        /// Peak torque value as calculated from the power curve.
        /// After changing power, power curve or RPM range call UpdatePeakPowerAndTorque() to get update the value.
        /// </summary>
        public float EstimatedPeakTorque
        {
            get { return _peakTorque; }
        }
        private float _peakTorque;


        /// <summary>
        /// RPM at which the engine achieves the peak torque, calculated from the power curve.
        /// After changing power, power curve or RPM range call UpdatePeakPowerAndTorque() to get update the value.
        /// </summary>
        public float EstimatedPeakTorqueRPM
        {
            get { return _peakTorqueRpm; }
        }
        private float _peakTorqueRpm;


        /// <summary>
        ///     RPM as a percentage of maximum RPM.
        /// </summary>
        public float RPMPercent
        {
            get { return _rpmPercent; }
        }

        private float _rpmPercent;


        /// <summary>
        ///     Engine throttle position. 0 for no throttle and 1 for full throttle.
        /// </summary>
        [ShowInTelemetry]
        public float ThrottlePosition
        {
            get { return _throttlePosition; }
        }

        private float _throttlePosition;


        /// <summary>
        /// Is the engine currently running?
        /// Requires ignition to be enabled and stall RPM above the stall RPM.
        /// </summary>
        public bool IsRunning
        {
            get { return _isRunning; }
        }
        private bool _isRunning;


        /// <summary>
        /// Is the engine stalled or stalling?
        /// ICE engine only.
        /// </summary>
        public bool IsStalled
        {
            get
            {
                return engineType != EngineType.Electric 
                    && stallingEnabled 
                    && outputAngularVelocity < _idleAngularVelocity * 0.4f;
            }
        }


        /// <summary>
        /// Current load of the engine, based on the power produced.
        /// </summary>
        public float Load
        {
            get { return _load; }
        }
        private float _load;
        


        protected override void VC_Initialize()
        {
            UpdatePeakPowerAndTorque();

            if (engineType == EngineType.ICE)
            {
                calculateTorqueDelegate = CalculateTorqueICE;
            }
            else if (engineType == EngineType.Electric)
            {
                forcedInduction.useForcedInduction = false;
                stallingEnabled = false;
                idleRPM = 0f;
                flyingStartEnabled = true;
                calculateTorqueDelegate = CalculateTorqueElectric;
                starterActive = false;
                startDuration = 0.001f;
                revLimiterCutoffDuration = 0f;
            }
            
            base.VC_Initialize();
        }


        public override bool VC_Enable(bool calledByParent)
        {
            if (base.VC_Enable(calledByParent))
            {
                if (flyingStartEnabled)
                {
                    StartEngine();
                }
                
                return true;
            }

            return false;
        }


        public override bool VC_Disable(bool calledByParent)
        {
            if (base.VC_Disable(calledByParent))
            {
                if (IsRunning)
                {
                    StopEngine();
                }

                if (_starterCoroutine != null && vehicleController != null)
                {
                    vehicleController.StopCoroutine(_starterCoroutine);
                }
                return true;
            }

            return false;
        }


        public override void VC_Validate(VehicleController vc)
        {
            base.VC_Validate(vc);

            if (engineType != EngineType.Electric && (powerCurve == null || powerCurve.keys.Length < 2))
            {
                PC_LogWarning(vc, "Power curve of the engine is not set.");
            }
        }


        public override void VC_SetDefaults()
        {
            base.VC_SetDefaults();

            inertia = 0.25f;
            forcedInduction = new ForcedInduction();
            powerCurve = new AnimationCurve
            {
                keys = new[]
                {
                    new Keyframe(0f,   0f,   0,  1f),
                    new Keyframe(1f,   1f)
                },
            };
            UpdatePeakPowerAndTorque();
            Output = vehicleController.powertrain.clutch;
        }


        public void UpdatePeakPowerAndTorque()
        {
            GetPeakPower(out _peakPower, out _peakPowerRpm);
            GetPeakTorque(out _peakTorque, out _peakTorqueRpm);
        }


        /// <summary>
        /// Toggles engine state.
        /// </summary>
        public void StartStopEngine()
        {
            if (IsRunning)
            {
                StopEngine();
            }
            else
            {
                StartEngine();
            }
        }


        /// <summary>
        ///     Calculates torque for electric engine type.
        /// </summary>
        public float CalculateTorqueElectric(float angularVelocity, float dt)
        {
            float absAngVel = Mathf.Abs(angularVelocity);

            // Avoid angular velocity spikes while shifting
            if (vehicleController.powertrain.transmission.isShifting)
            {
                _throttlePosition = 0;
            }
            
            float maxLossPower = maxPower * 0.3f;
            float lossPower = maxLossPower * (1f - _throttlePosition) * RPMPercent;
            float genPower = maxPower * _throttlePosition;
            float totalPower = genPower - lossPower;
            totalPower = Mathf.Lerp(totalPower * 0.1f, totalPower, RPMPercent * 10f);
            float clampedAngVel = absAngVel < 10f ? 10f : absAngVel;
            return PowerInKWToTorque(clampedAngVel, totalPower);
        }


        /// <summary>
        /// Calculates torque for ICE (Internal Combustion Engine).
        /// </summary>
        public float CalculateTorqueICE(float angularVelocity, float dt)
        {
            // Set the throttle to 0 when shifting, but avoid doing so around idle RPM to prevent stalls.
            if (vehicleController.powertrain.transmission.isShifting && angularVelocity > _idleAngularVelocity)
            {
                _throttlePosition = 0f;
            }

            // Set throttle to 0 when starter active.
            if (starterActive)
            {
                _throttlePosition = 0f;
            }
            // Apply idle throttle correction to keep the engine running
            else
            {
                ApplyICEIdleCorrection();
            }

            // Trigger rev limiter if needed
            if (angularVelocity >= _revLimiterAngularVelocity && !revLimiterActive)
            {
                vehicleController.StartCoroutine(RevLimiterCoroutine());
            }

            // Calculate torque
            float generatedTorque;
            
            // Do not generate any torque while starter is active to prevent RPM spike during startup
            // or while stalled to prevent accidental starts.
            if (starterActive || IsStalled)
            {
                generatedTorque = 0f;
            }
            else
            {
                generatedTorque = CalculateICEGeneratedTorqueFromPowerCurve();
            }
            
            float lossTorque = starterActive ? 0f : CalculateICELossTorqueFromPowerCurve();
            
            // Reduce the loss torque at rev limiter, but allow it to be >0 to prevent vehicle getting
            // stuck at rev limiter.
            if (revLimiterActive) lossTorque *= 0.25f;
            generatedTorque += _starterTorque + lossTorque;
            return generatedTorque;
        }


        private float CalculateICELossTorqueFromPowerCurve()
        {
            // Avoid issues with large torque spike around 0 angular velocity.
            if (outputAngularVelocity < 10f)
            {
                return -outputAngularVelocity * maxPower * 0.03f;
            }
        
            float angVelPercent = outputAngularVelocity < 10f ? 
                0.1f : 
                Mathf.Clamp(outputAngularVelocity, _stallAngularVelocity, _revLimiterAngularVelocity) / _revLimiterAngularVelocity;
            float lossPower = powerCurve.Evaluate(angVelPercent) * -maxPower * Mathf.Clamp01(_userThrottleInput + 0.5f) * engineLossPercent;
            return PowerInKWToTorque(outputAngularVelocity, lossPower);
        }


        private float CalculateICEGeneratedTorqueFromPowerCurve()
        {
            generatedPower = 0;
            float torque = 0;

            if (!ignition && !starterActive)
            {
                return 0;
            }
            
            if (revLimiterActive)
            {
                _throttlePosition = 0.2f;

            }
            else
            {
                // Add maximum losses to the maximum power when calculating the generated power since the maxPower is net value (after losses).
                generatedPower = powerCurve.Evaluate(_rpmPercent) * (maxPower * (1f + engineLossPercent)) * _throttlePosition
                    * _powerModifierSum * forcedInduction.PowerGainMultiplier;
                torque = PowerInKWToTorque(outputAngularVelocity, generatedPower);
            }

            return torque;
        }


        private void ApplyICEIdleCorrection()
        {
            if (ignition && outputAngularVelocity < _idleAngularVelocity * 1.1f)
            {
                float idleCorrection = _idleAngularVelocity - outputAngularVelocity;
                idleCorrection = idleCorrection < 0f ? 0f : idleCorrection;
                float idleThrottlePosition = Mathf.Clamp01(idleCorrection * 0.01f);
                _throttlePosition = Mathf.Max(_userThrottleInput, idleThrottlePosition);
            }
        }


        private IEnumerator RevLimiterCoroutine()
        {
            if (revLimiterActive || engineType == EngineType.Electric || revLimiterCutoffDuration == 0)
            {
                yield return null;
            }

            revLimiterActive = true;
            onRevLimiter.Invoke();
            yield return new WaitForSeconds(revLimiterCutoffDuration);
            revLimiterActive = false;
        }


        private IEnumerator StarterCoroutine()
        {
            if (engineType == EngineType.Electric || starterActive)
            {
                yield return null;
            }

            float startTimer = 0f;

            starterActive = true;

            // Calculate starter torque from the start duration and the inertia of the engine
            // Avoid start duration around 0 as that will apply large torque impulse.
            if (startDuration < 0.1f)
            {
                startDuration = 0.1f;
            }

            _starterTorque = ((_idleAngularVelocity - outputAngularVelocity) * inertia) / startDuration;
            
            // Run the starter
            while (startTimer <= startDuration)
            {
                startTimer += 0.1f;
                yield return new WaitForSeconds(0.1f);
            }

            // Start finished
            _starterTorque = 0;
            starterActive = false;
            
            // Check if engine running or start failed
            if (outputAngularVelocity < _stallAngularVelocity)
            {
                StopEngine();
            }
        }


        public void IntegrateDownwards(float dt)
        {
            // Cache values
            _userThrottleInput = vehicleController.input.InputSwappedThrottle;
            _throttlePosition = _userThrottleInput;
            _idleAngularVelocity = UnitConverter.RPMToAngularVelocity(idleRPM);
            _stallAngularVelocity = stallingEnabled ? _idleAngularVelocity * 0.4f : -1e10f;
            _revLimiterAngularVelocity = UnitConverter.RPMToAngularVelocity(revLimiterRPM);
            _prevAngularVelocity = outputAngularVelocity;
            
            // Check for start on throttle
            if (!IsRunning && !starterActive && autoStartOnThrottle && _throttlePosition > 0.2f && _damage < 0.999f)
            {
                StartEngine();
            }
            
            // Check for damage
            if (_prevDamage < 0.999f && _damage > 0.999f)
            {
                StopEngine();
            }
            _prevDamage = _damage;

            // Check for user start/stop input
            if (vehicleController.input.EngineStartStop)
            {
                StartStopEngine();
                vehicleController.input.EngineStartStop = false;
            }

            // Check if stall needed
            bool wasRunning = _isRunning;
            _isRunning = ignition && !IsStalled;
            if (wasRunning && !_isRunning)
            {
                StopEngine();
            }

            // Combine all the external power modifiers
            _powerModifierSum = SumPowerModifiers();

            // Update forced induction
            if (engineType != EngineType.Electric)
            {
                forcedInduction.Update(this);
            }

            // Physics update
            if (outputNameHash == 0)
            {
                return;
            }
            
            float drivetrainInertia = _output.QueryInertia();
            float inertiaSum = inertia + drivetrainInertia;
            float drivetrainAngularVelocity = QueryAngularVelocity(outputAngularVelocity, dt);
            float targetAngularVelocity = inertia / inertiaSum * outputAngularVelocity + drivetrainInertia / inertiaSum * drivetrainAngularVelocity;

            // Calculate generated torque and power
            float generatedTorque = calculateTorqueDelegate(outputAngularVelocity, dt);
            generatedPower = TorqueToPowerInKW(in outputAngularVelocity, in generatedTorque);

            // Calculate reaction torque
            float reactionTorque = (targetAngularVelocity - outputAngularVelocity) * inertia / dt;

            // Calculate/get torque returned from wheels
            outputTorque = generatedTorque - reactionTorque;
            float returnTorque = ForwardStep(outputTorque, 0, dt);

            float totalTorque = generatedTorque + returnTorque + reactionTorque;
            outputAngularVelocity += totalTorque / inertiaSum * dt;

            // Clamp the angular velocity to prevent any powertrain instabilities over the limits
            float minAngularVelocity = 0f;
            float maxAngularVelocity = _revLimiterAngularVelocity * 1.05f;
            outputAngularVelocity = Mathf.Clamp(outputAngularVelocity, minAngularVelocity, maxAngularVelocity);

            // Calculate cached values
            _rpmPercent = Mathf.Clamp01(outputAngularVelocity / _revLimiterAngularVelocity);
            _load = Mathf.Clamp01(generatedPower / maxPower);
        }



        /// <summary>
        ///     Starts the engine.
        /// </summary>
        public void StartEngine()
        {
            ignition = true;

            if (_damage >= 1f)
            {
                return;
            }

            onStart.Invoke();

            if (engineType != EngineType.Electric)
            {
                if (flyingStartEnabled)
                {
                    FlyingStart();
                }
                else if (!starterActive)
                {
                    if (vehicleController != null)
                    {
                        _starterCoroutine = vehicleController.StartCoroutine(StarterCoroutine());
                    }
                }
            }
        }


        private void FlyingStart()
        {
            ignition = true;
            starterActive = false;
            outputAngularVelocity = UnitConverter.RPMToAngularVelocity(idleRPM);
        }


        /// <summary>
        ///      Stops the engine.
        /// </summary>
        public void StopEngine()
        {
            ignition = false;
            onStop.Invoke();
        }

        

        private float SumPowerModifiers()
        {
            if (powerModifiers.Count == 0)
            {
                return 1f;
            }

            float coefficient = 1;
            int n = powerModifiers.Count;
            for (int i = 0; i < n; i++)
            {
                coefficient *= powerModifiers[i].Invoke();
            }

            return Mathf.Clamp(coefficient, 0f, Mathf.Infinity);
        }


        public void GetPeakTorque(out float peakTorque, out float peakTorqueRpm)
        {
            peakTorque = 0;
            peakTorqueRpm = 0;

            for (float i = 0.05f; i < 1f; i += 0.05f)
            {
                float rpm = i * revLimiterRPM;
                float P = powerCurve.Evaluate(i) * maxPower;
                if (rpm < idleRPM)
                {
                    continue;
                }
                float W = UnitConverter.RPMToAngularVelocity(rpm);
                float T = (P * 1000f) / W;

                if (T > peakTorque)
                {

                    peakTorque = T;
                    peakTorqueRpm = rpm;
                }
            }

            float fiCoefficient = forcedInduction.useForcedInduction ? forcedInduction.powerGainMultiplier : 1f;
            peakTorque = peakTorque * fiCoefficient;
        }


        public void GetPeakPower(out float peakPower, out float peakPowerRpm)
        {
            float maxY = 0f;
            float maxX = 1f;
            for (float i = 0f; i < 1f; i += 0.05f)
            {
                float y = powerCurve.Evaluate(i);
                if (y > maxY)
                {
                    maxY = y;
                    maxX = i;
                }
            }

            float fiCoefficient = forcedInduction.useForcedInduction ? forcedInduction.powerGainMultiplier : 1f;
            peakPower = maxY * maxPower * fiCoefficient;
            peakPowerRpm = maxX * revLimiterRPM;
        }
    }
}


#if UNITY_EDITOR
namespace NWH.VehiclePhysics2.Powertrain
{
    [CustomPropertyDrawer(typeof(EngineComponent))]
    public partial class EngineComponentDrawer : PowertrainComponentDrawer
    {
        private EngineComponent _engineComponent;

        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            DrawCommonProperties();

            _engineComponent =
                SerializedPropertyHelper.GetTargetObjectOfProperty(property) as EngineComponent;

            drawer.BeginSubsection("Type");
            drawer.Field("engineType", !Application.isPlaying);
            int typeEnumValue = property.FindPropertyRelative("engineType").enumValueIndex;
            drawer.EndSubsection();

            bool isElectric = typeEnumValue == (int)EngineComponent.EngineType.Electric;

            if (isElectric)
            {
                drawer.BeginSubsection("Power & Torque");
                drawer.Field("maxPower", true, "kW");
                drawer.Info("Electric EngineType is using constant torque to calculate power which is why there is no power curve " +
                            "setting.");
                drawer.Field("engineLossPercent");
                drawer.EndSubsection();
                
                drawer.BeginSubsection("");
                drawer.Field("ignition");
                drawer.Field("revLimiterRPM");
                drawer.EndSubsection();
            }
            else
            {
                drawer.BeginSubsection("General");
                if (Application.isPlaying)
                {
                    drawer.Label($"Is Running: {_engineComponent.IsRunning}");
                }
                drawer.Field("autoStartOnThrottle");
                drawer.Field("stallingEnabled");
                drawer.EndSubsection();

                drawer.BeginSubsection("Power & Torque");
                EditorGUI.BeginChangeCheck();
                drawer.Field("maxPower", true, "kW");
                drawer.Field("powerCurve");
                drawer.Info("Power and torque curves are showing the same information. That is why there is no additional torque curve. " +
                    "If you know one, you know the other.");
                drawer.Field("engineLossPercent");
                drawer.Space(5);

                _engineComponent.UpdatePeakPowerAndTorque();

                GUI.enabled = false;
                string fiString = _engineComponent.forcedInduction.useForcedInduction
                                      ? "(including +" + ((_engineComponent.forcedInduction.powerGainMultiplier - 1f) * 100f).ToString(
                                            "F0") + "% from FI)"
                                      : "";
                drawer.Label($"Peak Power:\t{_engineComponent.EstimatedPeakPower:F0}kw " +
                             $"@ {_engineComponent.EstimatedPeakPowerRPM:F0}RPM {fiString}");

                drawer.Label($"Peak Torque:\t{_engineComponent.EstimatedPeakTorque:F0}Nm " +
                    $"@ {_engineComponent.EstimatedPeakTorqueRPM:F0}RPM");
                GUI.enabled = true;

                drawer.EndSubsection();

                drawer.BeginSubsection("Starter");
                drawer.Field("ignition");
                drawer.Field("startDuration", true, "s");
                drawer.Field("flyingStartEnabled");
                drawer.EndSubsection();

                drawer.BeginSubsection("Idler Circuit");
                drawer.Field("idleRPM");
                drawer.EndSubsection();

                drawer.BeginSubsection("Rev Limiter");
                drawer.Field("revLimiterRPM");
                drawer.Field("revLimiterCutoffDuration");
                drawer.EndSubsection();

                drawer.BeginSubsection("Forced Induction");
                drawer.Property("forcedInduction");
                drawer.EndSubsection();
            }


            drawer.EndProperty();
            return true;
        }
    }
}

#endif
