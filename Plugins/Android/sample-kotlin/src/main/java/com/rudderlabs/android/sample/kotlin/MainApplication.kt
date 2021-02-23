package com.rudderlabs.android.sample.kotlin

import android.app.Application
import com.rudderstack.android.sdk.core.RudderLogger
import com.rudderstack.android.sdk.wrapper.RudderClientWrapper

class MainApplication : Application() {
    companion object {
        private const val WRITE_KEY = "1TSRSskqa15PG7F89tkwEbl5Td8"
        private const val DATA_PLANE_URL = "https://d87e3736.ngrok.io"
        private const val CONTROL_PLANE_URL = "https://d87e3736.ngrok.io"
    }

    override fun onCreate() {
        super.onCreate()

        RudderClientWrapper._initiateInstance(
            this,
            "some_anonymous_id",
            WRITE_KEY,
            DATA_PLANE_URL,
            CONTROL_PLANE_URL,
            30,
            10000,
            10,
            2,
            true,
            true,
            RudderLogger.RudderLogLevel.VERBOSE
        )
    }
}