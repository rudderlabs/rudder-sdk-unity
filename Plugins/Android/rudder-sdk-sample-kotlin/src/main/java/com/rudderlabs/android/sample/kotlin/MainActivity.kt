package com.rudderlabs.android.sample.kotlin

import android.os.Bundle
import androidx.appcompat.app.AppCompatActivity
import com.google.gson.Gson
import com.rudderlabs.android.sdk.core.RudderClientWrapper
import com.rudderlabs.android.sdk.core.RudderTraits
import kotlinx.android.synthetic.main.activity_main.*

class MainActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        val eventProps = mapOf(
            "test_key_1" to "test_value_1",
            "test_key_2" to "test_value_2",
            "price" to 2.9999,
            "quantity" to 1
        )
        val eventJson = Gson().toJson(eventProps)
        val userProps = mapOf(
            "test_user_key_1" to "test_user_value_1",
            "test_user_key_2" to "test_user_value_2"
        )
        val userJson = Gson().toJson(userProps)

        val optionsJson = "null"

        trackBtn.setOnClickListener {
            RudderClientWrapper._logEvent(
                "track",
                "some_test_event",
                eventJson,
                userJson,
                optionsJson
            )
        }

        screenBtn.setOnClickListener {
            RudderClientWrapper._logEvent(
                "screen",
                "some_test_event",
                eventJson,
                userJson,
                optionsJson
            )
        }

        identifyBtn.setOnClickListener {
            val traits: RudderTraits = RudderTraits()
                .putAddress(
                    RudderTraits.Address()
                        .putCity("some_city")
                        .putCountry("some_country")
                        .putPostalCode("123456")
                        .putState("some_state")
                        .putStreet("some_street")
                )
                .put("userId", userId.text.toString())
                .put("some_test_key", "some_test_value")
            val traitsJson = Gson().toJson(traits)

            RudderClientWrapper._identify(
                "some_user_id",
                traitsJson,
                optionsJson
            )
        }

        resetBtn.setOnClickListener {
            RudderClientWrapper._reset()
        }

    }

//    private fun sendRevenueEvent() {
//        val eventType = "track"
//        val eventName = "revenue"
//        val userId = "test_user_id"
//
//        val integrations = mapOf(
//            "All" to false,
//            "AM" to true,
//            "GA" to true
//        )
//        val integrationJson = Gson().toJson(integrations)
//
//        RudderClientWrapper._logEvent(
//            eventType,
//            eventName,
//            eventJson,
//            userJson,
//            "null"
//        )
//    }
//
//    private fun sendGenericTrackEvent() {
//        val eventType = "track"
//        val eventName = "level_up"
//        val userId = "test_user_id"
//        val eventProps = mapOf(
//            "test_key_1" to "test_value_1",
//            "test_key_2" to "test_value_2"
//        )
//        val eventJson = Gson().toJson(eventProps)
//        val userProps = mapOf(
//            "test_user_key_1" to "test_user_value_1",
//            "test_user_key_2" to "test_user_value_2"
//        )
//        val userJson = Gson().toJson(userProps)
//
//        RudderClientWrapper._logEvent(
//            eventType,
//            eventName,
//            eventJson,
//            userJson,
//            "null"
//        )
//    }
}
