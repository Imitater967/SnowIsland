using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace SnowIsland.Scripts
{
    public static class LeakDetectionControl  
    {
        [MenuItem("Jobs/LeakDetection/Enable")]
        private static void LeakDetection()
        {
            NativeLeakDetection.Mode=NativeLeakDetectionMode.Enabled;
        }        
        
        [MenuItem("Jobs/LeakDetection/EnableWithStackTrace")]
        private static void LeakDetectionWithStackTrace()
        {
            NativeLeakDetection.Mode=NativeLeakDetectionMode.EnabledWithStackTrace;
        }
        
        [MenuItem("Jobs/LeakDetection/Disable")]
        private static void NoLeakDetection()
        {
            NativeLeakDetection.Mode=NativeLeakDetectionMode.Disabled;
        }
    }
}