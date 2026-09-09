# MobileHapticFeedback is invoked from Unity via reflection (AndroidJavaClass),
# so R8 cannot see any references to it. Keep the public entry points.
-keep class com.tsyk5.mobilehapticfeedback.MobileHapticFeedback { public *; }
