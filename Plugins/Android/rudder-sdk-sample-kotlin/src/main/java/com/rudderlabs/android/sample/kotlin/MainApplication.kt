package com.rudderlabs.android.sample.kotlin

import android.app.Application
import com.rudderlabs.android.sdk.core.RudderClientWrapper
import com.rudderlabs.android.sdk.core.RudderLogger

class MainApplication : Application() {
    companion object {
        private const val WRITE_KEY = "1QH0xOiRBxiKht41YBe90rJVVXR"
        private const val END_POINT_URI = "https://c94d7782.ngrok.io"
    }

    override fun onCreate() {
        super.onCreate()

        RudderClientWrapper._initiateInstance(
            this,
            WRITE_KEY,
            END_POINT_URI,
            30,
            10000,
            10,
            RudderLogger.RudderLogLevel.VERBOSE
        )
    }
}