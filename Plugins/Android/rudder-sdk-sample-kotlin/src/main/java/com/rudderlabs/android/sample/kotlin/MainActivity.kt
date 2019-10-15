package com.rudderlabs.android.sample.kotlin

import android.os.Bundle
import androidx.appcompat.app.AppCompatActivity
import com.rudderlabs.android.sdk.core.*
import com.rudderlabs.android.sdk.ecomm.*
import com.rudderlabs.android.sdk.ecomm.events.*
import kotlinx.android.synthetic.main.activity_main.*
import kotlin.random.Random

class MainActivity : AppCompatActivity() {
    private var count = 0

    private fun createProduct(): ECommerceProduct {
        val rand = "${System.currentTimeMillis()}_${Random.nextInt(0, 100)}"
        return ECommerceProduct.Builder()
            .withProductId("test_product_id_$rand")
            .withBrand("Dummy Brand $rand")
            .withCategory("Dummy Category")
            .withCurrency("USD")
            .withName("Dummy Product Name $rand")
            .withPrice(10f)
            .withQuantity(1f)
            .build()
    }

    fun createCart(): ECommerceCart {
        val rand = "${System.currentTimeMillis()}_${Random.nextInt(0, 100)}"
        return ECommerceCart.Builder()
            .withCartId("dummy_cart_id_$rand")
            .withProducts(createProduct(), createProduct(), createProduct())
            .build()
    }

    fun createCoupon(): ECommerceCoupon {
        val rand = "${System.currentTimeMillis()}_${Random.nextInt(0, 100)}"
        return ECommerceCoupon.Builder()
            .withCartId("dummy_cart_id_$rand")
            .withCouponId("dummy_coupon_id_$rand")
            .withCouponName("Dummy Coupon Name $rand")
            .withOrderId("test_order_id_$rand")
            .withDiscount(5f)
            .withReason("Test Reason")
            .build()
    }

    fun createFilter(): ECommerceFilter {
        val rand = "${System.currentTimeMillis()}_${Random.nextInt(0, 100)}"
        return ECommerceFilter.Builder()
            .withType("Test Filter Type $rand")
            .withValue("Test Filter Value $rand")
            .build()
    }

    fun createSort(): ECommerceSort {
        val rand = "${System.currentTimeMillis()}_${Random.nextInt(0, 100)}"
        return ECommerceSort.Builder()
            .withType("Test Sort Type $rand")
            .withValue("Test Sort Value $rand")
            .build()
    }

    fun createOrder(): ECommerceOrder {
        val rand = "${System.currentTimeMillis()}_${Random.nextInt(0, 100)}"
        return ECommerceOrder.Builder()
            .withAffiliation("Test Affiliation $rand")
            .withCoupon("Dummy Coupon $rand")
            .withCurrency("USD")
            .withDiscount(5f)
            .withOrderId("test_order_id_$rand")
            .withProducts(createProduct(), createProduct(), createProduct())
            .withRevenue(40f)
            .withShippingCost(10f)
            .withTax(5f)
            .withTotal(50f)
            .withValue(45f)
            .build()
    }

    fun createPromotion(): ECommercePromotion {
        val rand = "${System.currentTimeMillis()}_${Random.nextInt(0, 100)}"
        return ECommercePromotion.Builder()
            .withCreative("Test Creative $rand")
            .withName("Test Name $rand")
            .withPosition("top_banner")
            .withPromotionId("test_promotion_id_$rand")
            .build()
    }

    fun createWishList(): ECommerceWishList {
        val rand = "${System.currentTimeMillis()}_${Random.nextInt(0, 100)}"
        return ECommerceWishList.Builder()
            .withWishListId("test_wish_list_id_$rand")
            .withWishListName("Test Wish List Name $rand")
            .build()
    }

    fun createCheckout(): ECommerceCheckout {
        val rand = "${System.currentTimeMillis()}_${Random.nextInt(0, 100)}"
        return ECommerceCheckout.Builder()
            .withCheckoutId("test_checkout_id_$rand")
            .withOrderId("test_order_id_$rand")
            .withPaymentMethod("Visa")
            .withShippingMethod("Fedex")
            .withStep(Random.nextInt(0, 10))
            .build()
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        btn.setOnClickListener {
            MainApplication.rudderEcommClient.client.track(
                RudderElementBuilder()
                    .setEventName("test_event")
                    .setProperty(
                        TrackPropertyBuilder()
                            .setCategory("test_category")
                            .setLabel("test label")
                            .setValue("test value")
                            .build()
                    )
                    .setUserId("test_user_id")
            )

            MainApplication.rudderEcommClient.client.screen(
                RudderElementBuilder()
                    .setEventName("test_event")
                    .setProperty(
                        ScreenPropertyBuilder()
                            .setScreenName("home screen")
                            .build()
                    )
                    .setUserId("test_user_id")
            )

            MainApplication.rudderEcommClient.client.page(
                RudderElementBuilder()
                    .setEventName("test_event")
                    .setProperty(
                        PagePropertyBuilder()
                            .setKeywords("test keyword 1")
                            .setPath("Test Path")
                            .setReferrer("Test referrer")
                            .setSearch("Test Search")
                            .setTitle("Test Title")
                            .setUrl("http://example.com/some/dummy/url")
                    )
                    .setUserId("test_user_id")
            )

            MainApplication.rudderEcommClient.client.identify(
                RudderTraitsBuilder()
                    .setAge(24)
                    .setBirthDay("1992-06-06")
                    .setCity("NY")
                    .setCompanyId("test_company_id")
                    .setCompanyName("Test Company")
                    .setCountry("USA")
                    .setCreateAt("2019-09-09T08:08:08.000Z")
                    .setDescription("Test Description")
                    .setEmail("example@test.com")
                    .setFirstName("First Name")
                    .setGender("Male")
                    .setId("test_user_id")
                    .setIndustry("Test Industry")
                    .setLastName("Last Name")
                    .setName("First Last")
                    .setPhone("9876543210")
                    .setPostalCode("ZA98090")
                    .setState("TEST")
                    .setTitle("Mr.")
                    .setUserName("Test User Name")
                    .setCity("New York")
            )

            MainApplication.rudderEcommClient.track(
                CartSharedEvent()
                    .withCart(createCart())
                    .withRecipient("friend@gmail.com")
                    .withShareMessage("Test Share Message")
                    .withSocialChannel("Email")
            )

            MainApplication.rudderEcommClient.track(
                CartViewedEvent()
                    .withCart(createCart())
            )

            MainApplication.rudderEcommClient.track(
                CheckoutStartedEvent()
                    .withCheckout(createCheckout())
                    .withOrder(createOrder())
            )

            MainApplication.rudderEcommClient.track(
                CheckoutStepCompletedEvent()
                    .withCheckout(createCheckout())
            )

            MainApplication.rudderEcommClient.track(
                CheckoutStepViewedEvent()
                    .withCheckout(createCheckout())
            )

            MainApplication.rudderEcommClient.track(
                CouponAppliedEvent()
                    .withCoupon(createCoupon())
            )

            MainApplication.rudderEcommClient.track(
                CouponDeniedEvent()
                    .withCoupon(createCoupon())
            )

            MainApplication.rudderEcommClient.track(
                CouponEnteredEvent()
                    .withCoupon(createCoupon())
            )

            MainApplication.rudderEcommClient.track(
                CouponRemovedEvent()
                    .withCoupon(createCoupon())
            )

            MainApplication.rudderEcommClient.track(
                OrderCancelledEvent()
                    .withOrder(createOrder())
            )

            MainApplication.rudderEcommClient.track(
                OrderCompletedEvent()
                    .withOrder(createOrder())
            )

            MainApplication.rudderEcommClient.track(
                OrderCompletedEvent()
                    .withOrder(createOrder())
            )

            MainApplication.rudderEcommClient.track(
                OrderRefundedEvent()
                    .withOrder(createOrder())
                    .withRefundValue(40f)
            )

            MainApplication.rudderEcommClient.track(
                OrderRefundedEvent()
                    .withOrder(createOrder())
                    .withProducts(createProduct(), createProduct())
                    .withRefundValue(40f)
            )

            MainApplication.rudderEcommClient.track(
                OrderUpdatedEvent()
                    .withOrder(createOrder())
            )

            MainApplication.rudderEcommClient.track(
                PaymentInfoEnteredEvent()
                    .withCheckout(createCheckout())
            )

            MainApplication.rudderEcommClient.track(
                ProductAddedToCartEvent()
                    .withCartId(createCart().cartId)
                    .withProduct(createProduct())
            )

            MainApplication.rudderEcommClient.track(
                ProductAddedToWishListEvent()
                    .withWishList(createWishList())
                    .withProduct(createProduct())
            )

            MainApplication.rudderEcommClient.track(
                ProductClickedEvent()
                    .withProduct(createProduct())
            )

            MainApplication.rudderEcommClient.track(
                ProductListFilteredEvent()
                    .withCategory(createProduct().category)
                    .withFilters(createFilter(), createFilter())
                    .withSorts(createSort(), createSort())
                    .withListId("test_sorted_id")
                    .withProducts(createProduct(), createProduct(), createProduct())
            )

            MainApplication.rudderEcommClient.track(
                ProductListViewedEvent()
                    .withCategory("Test Category")
                    .withListId("test_list_id")
                    .withProducts(createProduct(), createProduct(), createProduct())
            )

            MainApplication.rudderEcommClient.track(
                ProductRemovedEvent()
                    .withProduct(createProduct())
            )

            MainApplication.rudderEcommClient.track(
                ProductRemovedFromWishListEvent()
                    .withWishList(createWishList())
                    .withProduct(createProduct())
            )

            MainApplication.rudderEcommClient.track(
                ProductReviewedEvent()
                    .withProduct(createProduct())
                    .withRating("4.0")
                    .withReviewBody("Test Review Body")
                    .withReviewId("test_review_id")
            )

            MainApplication.rudderEcommClient.track(
                ProductSearchedEvent()
                    .withQuery("blue hotpants")
            )

            MainApplication.rudderEcommClient.track(
                ProductSharedEvent()
                    .withProduct(createProduct())
                    .withRecipient("friend@gmail.com")
                    .withShareMessage("Test Share Message")
                    .withSocialChannel("Email")
            )

            MainApplication.rudderEcommClient.track(
                ProductViewedEvent()
                    .withProduct(createProduct())
            )

            MainApplication.rudderEcommClient.track(
                PromotionClickedEvent()
                    .withPromotion(createPromotion())
            )

            MainApplication.rudderEcommClient.track(
                PromotionViewedEvent()
                    .withPromotion(createPromotion())
            )

            MainApplication.rudderEcommClient.track(
                WishListProductAddedToCartEvent()
                    .withCart(createCart())
                    .withProduct(createProduct())
                    .withWishList(createWishList())
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
