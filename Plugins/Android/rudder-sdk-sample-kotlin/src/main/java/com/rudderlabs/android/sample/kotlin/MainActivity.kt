package com.rudderlabs.android.sample.kotlin

import android.os.Bundle
import androidx.appcompat.app.AppCompatActivity
import com.google.gson.Gson
import com.rudderlabs.android.sdk.core.RudderClient
import kotlinx.android.synthetic.main.activity_main.*

class MainActivity : AppCompatActivity() {
    private var count = 0

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        btn.setOnClickListener {
            val eventType = "track"
            val eventName = "test_track_unity_android"
            val userId = "test_user_id"
            val eventProps = mapOf("test_key_1" to "test_value_1", "test_key_2" to "test_value_2")
            val eventJson = Gson().toJson(eventProps)
            val userProps = mapOf("test_user_key_1" to "test_user_value_1", "test_user_key_2" to "test_user_value_2")
            val userJson = Gson().toJson(userProps)
            val integrations = mapOf("All" to false, "GA" to false, "AM" to true)
            val integrationJson = Gson().toJson(integrations)

            RudderClient._logEvent(
                eventType,
                eventName,
                userId,
                eventJson,
                userJson,
                integrationJson
            )

            count += 1
            textView.text = "Count: $count"
        }

        rst.setOnClickListener {
            count = 0
            textView.text = "Count: "
        }
    }
}
