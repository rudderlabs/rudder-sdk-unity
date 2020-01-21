package com.rudderlabs.android.sample.kotlin

import android.app.Application
import com.rudderlabs.android.sdk.core.RudderClientWrapper
import com.rudderlabs.android.sdk.core.RudderLogger

class MainApplication : Application() {
    companion object {
        private const val WRITE_KEY = "1TSRSskqa15PG7F89tkwEbl5Td8"
        private const val END_POINT_URI = "https://76aeb180.ngrok.io"
    }

    override fun onCreate() {
        super.onCreate()

        RudderClientWrapper._initiateInstance(
            this,
            "some_anonymous_id",
            WRITE_KEY,
            END_POINT_URI,
            30,
            10000,
            10,
            RudderLogger.RudderLogLevel.VERBOSE
        )
    }
}