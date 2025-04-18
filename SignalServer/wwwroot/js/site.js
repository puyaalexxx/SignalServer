﻿// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


//import {signalR} from "../lib/signalr/dist/browser/signalr";

const connection = new signalR.HubConnectionBuilder() .withUrl("/learningHub", {
    transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling, 
    headers: { "Key": "value" }, 
    accessTokenFactory: null, 
    logMessageContent: true, 
    skipNegotiation: false, 
    withCredentials: true, 
    timeout: 100000
})
.configureLogging(signalR.LogLevel.Information)
.build();

connection.serverTimeoutInMilliseconds = 30000;
connection.keepAliveIntervalInMilliseconds = 15000;

connection.on("ReceiveMessage", message => {
    $("#signalr-message-panel").prepend($('<div />').text(message));
})

//send to all
$('#btn-broadcast').on('click', e => {
    //let message = $('#broadcast').val();
    //connection.invoke("BroadcastMessage", message).catch((err) => console.error(err.toString()));

    //streaming
    let message = $('#broadcast').val();
    if (message.includes(';')) {
        let messages = message.split(';');
        let subject = new signalR.Subject();
        
        connection.send("BroadcastStream", subject).catch(err => console.error(err.toString()));
        
        for (let i = 0; i < messages.length; i++) { subject.next(messages[i]);
        }
        subject.complete();
    } else {
        connection.invoke("BroadcastMessage", message).catch(err => console.error(err.toString()));
    }
})

//send to other clients
$('#btn-others-message').on('click', e => {
    let message = $('#others-message').val();
    connection.invoke("SendToOthers", message).catch((err) => console.error(err.toString()));
})

//send to the self
$('#btn-self-message').on('click', e => {
    let message = $('#self-message').val();
    connection.invoke("SendToCaller", message).catch((err) => console.error(err.toString()));
})

//send to specific client by their id
$('#btn-individual-message').on('click', e => {
    let message = $('#individual-message').val();
    let connectionID = $('#connection-for-message').val();
    connection.invoke("SendToIndividual", connectionID, message).catch((err) => console.error(err.toString()));
})

//send to a group of clients
$('#btn-group-message').click(function () {
    let message = $('#group-message').val();
    let group = $('#group-for-message').val();
    connection.invoke("SendToGroup", group, message).catch(err => console.error(err.toString()));
});
$('#btn-group-add').click(function () {
    let group = $('#group-to-add').val();
    connection.invoke("AddUserToGroup", group).catch(err => console.error(err.toString())); });
$('#btn-group-remove').click(function () {
    let group = $('#group-to-remove').val();
    connection.invoke("RemoveUserFromGroup", group).catch(err => console.error(err.toString()));
});

//server streaming
$('#btn-trigger-stream').click(function () {
    let numberOfJobs = parseInt($('#number-of-jobs').val(), 10);
    
    connection.stream("TriggerStream", numberOfJobs)
        .subscribe({
            next: (message) => $('#signalr-message-panel').prepend($('<div />').text(message))
        }); 
});

async function start(){
    try {
        await connection.start();
        console.log("connected");
    } catch (error) {
        console.log(error);
        setTimeout(() => start(), 5000);
    }
}

connection.onclose(async () => {
    await start();
});

start().then(r => {});