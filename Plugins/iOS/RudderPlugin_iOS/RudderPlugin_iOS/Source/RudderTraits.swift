//
//  RudderTraits.swift
//  RudderSample
//
//  Created by Arnab Pal on 12/07/19.
//  Copyright © 2019 Arnab Pal. All rights reserved.
//

import Foundation

struct RudderTraits: Encodable {
    var anonymousId: String
    var address: TraitsAddress? = nil
    var age: Int? = nil
    var birthday: String? = nil
    var company: TraitsCompany? = nil
    var createdAt: String? = nil
    var description: String? = nil
    var email: String? = nil
    var firstName: String? = nil
    var gender: String? = nil
    var id: String? = nil
    var lastName: String? = nil
    var name: String? = nil
    var phone: String? = nil
    var title: String? = nil
    var userName: String? = nil
    
    enum CodingKeys: String, CodingKey {
        case anonymousId =  "rl_anonymous_id"
        case address = "rl_address"
        case age = "rl_age"
        case birthday = "rl_birthday"
        case company = "rl_company"
        case createdAt = "rl_createdat"
        case description = "rl_description"
        case email = "rl_email"
        case firstName = "rl_firstname"
        case gender = "rl_gender"
        case id = "rl_id"
        case lastName = "rl_lastname"
        case name = "rl_name"
        case phone = "rl_phone"
        case title = "rl_title"
        case userName = "rl_username"
    }
    
    init(anonymousId: String) {
        self.anonymousId = anonymousId
    }
    
    init(address: TraitsAddress, age: Int, birthday: String, company: TraitsCompany, createdAt: String, description: String, email: String, firstName: String, gender: String, id: String, lastName: String, name: String, phone: String, title: String, userName: String) {
        self.anonymousId = RudderElementCache.getCachedContext().deviceInfo.id
        self.address = address
        self.age = age
        self.birthday = birthday
        self.company = company
        self.createdAt = createdAt
        self.description = description
        self.email = email
        self.firstName = firstName
        self.gender = gender
        self.id = id
        self.lastName = lastName
        self.name = name
        self.phone = phone
        self.title = title
        self.userName = userName
    }
}
