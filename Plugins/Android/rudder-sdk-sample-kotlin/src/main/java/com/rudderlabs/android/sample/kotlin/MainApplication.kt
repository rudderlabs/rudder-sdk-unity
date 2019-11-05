package com.rudderlabs.android.sample.kotlin

import android.app.Application
import com.rudderlabs.android.sdk.core.RudderClient

class MainApplication : Application() {
    companion object {
        private const val WRITE_KEY = "1SuZEl2bawQm274slpZs9y6NdCi"
        private const val END_POINT_URI = "https://4d3fc588.ngrok.io"
    }

    override fun onCreate() {
        super.onCreate()

        RudderClient._initiateInstance(
            this,
            WRITE_KEY,
            END_POINT_URI,
            30,
            10000,
            10
        )
    }
}