//
//  EventRepository.swift
//  RudderPlugin_iOS
//
//  Created by Arnab Pal on 14/09/19.
//  Copyright © 2019 Arnab Pal. All rights reserved.
//

import Foundation

class EventRepository {
    private static var instance: EventRepository? = nil
    
    private var writeKey: String = ""
    private var endPointUrl: String = Constants.BASE_URL
    private var flushQueueSize: Int32 = Constants.FLUSH_QUEUE_SIZE
    private var dbCountThreshold: Int32 = Constants.DB_COUNT_THRESHOLD
    private var sleepTimeOut: Int32 = Constants.SLEEP_TIME_OUT
    
    private var dbManager: DBPersistentManager? = nil
    
    private init (_writeKey: String, _endPointUrl: String, _flushQueueSize: Int32, _dbCountThreshold: Int32, _sleepTimeOut: Int32) {
        
        self.writeKey = _writeKey
        if (_endPointUrl.last != "/") {
            self.endPointUrl = _endPointUrl + "/"
        } else {
            self.endPointUrl = _endPointUrl
        }
        self.endPointUrl = _endPointUrl
        self.flushQueueSize = _flushQueueSize
        self.dbCountThreshold = _dbCountThreshold
        self.sleepTimeOut = _sleepTimeOut
        
        RudderElementCache.initiate()
        
        dbManager = DBPersistentManager.getInstance()
        
        if #available(iOS 10.0, *) {
            let processorThread: Thread = Thread(block: processEvents)
            processorThread.start()
        }
    }
    
    private func processEvents() {
        if (self.dbManager == nil) {
            return
        }
        
        var sleepCount: Int32 = 0
        
        while true {
            let dbRecordCount = self.dbManager!.getDBRecordCount()
            if (dbRecordCount > self.dbCountThreshold) {
                let extraMsgs: RudderDBMessage = self.dbManager!.fetchEventsFromDB(count: dbRecordCount-self.dbCountThreshold)
                self.dbManager?.clearEventsFromDB(messageIds: extraMsgs.messageIds)
            }
            let dbMessages = self.dbManager!.fetchEventsFromDB(count: self.flushQueueSize)
            if (dbMessages.messages.count > 0 && sleepCount>=self.sleepTimeOut) {
                let payload: String = self.getPayloadFromMessages(message: dbMessages.messages)
                let response = self.flushEventsToServer(payload: payload)
                if (response == nil) {
                    // print("response: nil")
                } else {
                    print("response: " + response! + " || count: " + String(dbMessages.messages.count))
                }
                if (response != nil && response!.elementsEqual("OK")) {
                    // print("response matched")
                    self.dbManager!.clearEventsFromDB(messageIds: dbMessages.messageIds)
                    sleepCount = 0
                }
            }
            sleepCount += 1
            usleep(1000000)
        }
    }
    
    private func getPayloadFromMessages(message: [String]) -> String {
        var payload: String = ""
        
        payload.append("{")
        
        payload.append("\"sent_at\":\"")
        payload.append(Utils.getTimeStampStr())
        payload.append("\",")
        
        payload.append("\"batch\":[")
        for index in 0..<message.count {
            payload.append(message[index])
            if (index != message.count-1) {
                payload.append(",")
            }
        }
        payload.append("],")
        
        payload.append("\"writeKey\":\"")
        payload.append(self.writeKey)
        payload.append("\"")
        
        payload.append("}")
        
        return payload
    }
    
    private func flushEventsToServer(payload: String) -> String? {
        let semaphore = DispatchSemaphore(value: 0)
        
        let endPointUrl = self.endPointUrl + "/hello"
        
        let url = URL(string: endPointUrl)
        var urlRequest = URLRequest(url: url!)
        
        urlRequest.addValue("application/json", forHTTPHeaderField: "Content-Type")
        urlRequest.httpMethod = "POST"
        urlRequest.httpBody = Data(payload.utf8)
        
        var response: String? = nil
        let task = URLSession.shared.dataTask(with: urlRequest) {(data, result, error) in
            if (data != nil) {
                response = String(data: data!, encoding: String.Encoding.utf8)
            }
            semaphore.signal()
        }
        
        task.resume()
        semaphore.wait()
        
        return response
    }
    
    static func getInstance(writeKey: String, endPointUrl: String, flushQueueSize: Int32, dbCountThreshold: Int32, sleepTimeOut: Int32) -> EventRepository {
        if (instance == nil) {
            instance = EventRepository(_writeKey: writeKey, _endPointUrl: endPointUrl, _flushQueueSize: flushQueueSize, _dbCountThreshold: dbCountThreshold, _sleepTimeOut: sleepTimeOut)
        }
        return instance!
    }
    
    func dump(element: RudderElement) throws {
        let encoder = JSONEncoder()
        let eventJson = try encoder.encode(element)
        let eventString = String(data: eventJson, encoding: .utf8)
        // print("eventString: ", eventString!)
        self.dbManager!.saveEvent(messageJson: eventString!)
    }
}
