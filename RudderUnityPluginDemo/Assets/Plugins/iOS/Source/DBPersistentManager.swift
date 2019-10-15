//
//  DBPersistentManager.swift
//  RudderPlugin_iOS
//
//  Created by Arnab Pal on 14/09/19.
//  Copyright © 2019 Arnab Pal. All rights reserved.
//

import Foundation
import SQLite3

class DBPersistentManager {
    private static var instance: DBPersistentManager? = nil
    private var dbInstance: OpaquePointer? = nil
    
    static func getInstance() -> DBPersistentManager {
        if (instance == nil) {
            instance = DBPersistentManager()
        }
        return instance!
    }
    
    private init() {
        createDB()
        createSchema()
    }
    
    private func createDB() {
        if (dbInstance == nil) {
            sqlite3_shutdown()
            
            if (sqlite3_open_v2(Utils.getPath(fileName: "rl_persistance.db"), &self.dbInstance, SQLITE_OPEN_CREATE | SQLITE_OPEN_READWRITE | SQLITE_OPEN_FULLMUTEX, nil) == SQLITE_OK) {
                // print("Successfully opened connection to database.")
            } else {
                // print("Unable to open database.")
            }
        }
    }
 
    private func createSchema() {
        let createTableString = """
            CREATE TABLE IF NOT EXISTS events(
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            message TEXT NOT NULL,
            updated INTEGER NOT NULL);
        """
        var createTableStatement: OpaquePointer? = nil
        if sqlite3_prepare_v2(self.dbInstance, createTableString, -1, &createTableStatement, nil) == SQLITE_OK {
            if sqlite3_step(createTableStatement) == SQLITE_DONE {
                // print("Schema created.")
            } else {
                // print("Schema could not be created.")
            }
        } else {
            // print("CREATE TABLE statement could not be prepared.")
        }
        sqlite3_finalize(createTableStatement)
    }
    
    func saveEvent(messageJson: String) {
        let insertStatementString = "INSERT INTO events (message, updated) VALUES ('"+messageJson+"', "+String(Utils.gettimeStampLong())+");"
        // print("insertStatementString:", insertStatementString)
        var insertStatement: OpaquePointer? = nil
        if sqlite3_prepare_v2(self.dbInstance, insertStatementString, -1, &insertStatement, nil) == SQLITE_OK {
            if sqlite3_step(insertStatement) == SQLITE_DONE {
                // print("Successfully inserted row.")
            } else {
                // print("Could not insert row.")
            }
        } else {
            // print("INSERT statement could not be prepared.")
        }
        sqlite3_finalize(insertStatement)
    }
    
    func clearEventsFromDB(messageIds: [Int32]) {
        var messageIdCsv = ""
        for index in 0..<messageIds.count {
            messageIdCsv.append(String(messageIds[index]))
            if (index != messageIds.count-1) {
                messageIdCsv.append(contentsOf: ",")
            }
        }
        let deleteStatementStirng = "DELETE FROM events WHERE id IN ("+messageIdCsv+");"
        // print("deleteStatementStirng: ", deleteStatementStirng)
        var deleteStatement: OpaquePointer? = nil
        if sqlite3_prepare_v2(self.dbInstance, deleteStatementStirng, -1, &deleteStatement, nil) == SQLITE_OK {
            if sqlite3_step(deleteStatement) == SQLITE_DONE {
                // print("Successfully deleted row.")
            } else {
                // print("Could not delete row.")
            }
        } else {
            // print("DELETE statement could not be prepared")
        }
        sqlite3_finalize(deleteStatement)
    }
    
    func fetchEventsFromDB(count: Int32) -> RudderDBMessage {
        let queryStatementString = "SELECT * FROM events ORDER BY updated ASC LIMIT "+String(count)
        
        // print("queryStatementString: ", queryStatementString)
        
        var messageIds: [Int32] = []
        var messages: [String] = []
        
        var queryStatement: OpaquePointer? = nil
        if sqlite3_prepare_v2(self.dbInstance, queryStatementString, -1, &queryStatement, nil) == SQLITE_OK {
            while (sqlite3_step(queryStatement) == SQLITE_ROW) {
                let messageId = sqlite3_column_int(queryStatement, 0)
                let queryResultCol1 = sqlite3_column_text(queryStatement, 1)
                let message = String(cString: queryResultCol1!)
                messageIds.append(messageId)
                messages.append(message)
            }
            
            // print("SelectEvent count: ", messageIds.count)
        } else {
            // print("SELECT statement could not be prepared")
        }
        sqlite3_finalize(queryStatement)
        
        return RudderDBMessage(messageIds: messageIds, messages: messages)
    }
    
    func getDBRecordCount() -> Int32 {
        var count: Int32 = 0
        
        let countQuereStatementString = "SELECT COUNT(*) FROM 'events';"
        // print("countQuereStatementString: ", countQuereStatementString)
    
        var queryStatement: OpaquePointer? = nil
        if sqlite3_prepare_v2(self.dbInstance, countQuereStatementString, -1, &queryStatement, nil) == SQLITE_OK {
            while (sqlite3_step(queryStatement) == SQLITE_ROW) {
                count = sqlite3_column_int(queryStatement, 0)
            }
        } else {
            // print("SELECT statement could not be prepared")
        }
        sqlite3_finalize(queryStatement)
        
        return count
    }
}
