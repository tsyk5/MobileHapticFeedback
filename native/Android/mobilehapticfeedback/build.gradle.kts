plugins {
    id("com.android.library")
}

android {
    namespace = "com.tsyk5.mobilehapticfeedback"
    compileSdk = 34

    defaultConfig {
        minSdk = 21
    }

    buildTypes {
        release { isMinifyEnabled = false }
        debug { isMinifyEnabled = false }
    }

    compileOptions {
        sourceCompatibility = JavaVersion.VERSION_17
        targetCompatibility = JavaVersion.VERSION_17
    }
}

dependencies {
}
