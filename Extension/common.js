'use strict';

function notify(message) {
  chrome.notifications.create({
    title: chrome.runtime.getManifest().name,
    type: 'basic',
    iconUrl: 'data/icons/48.png',
    message
  });
}

function onClicked(tab) {
  chrome.tabs.executeScript(tab.id, {
    file: 'data/inject/Readability.js'
  }, () => {
    if (chrome.runtime.lastError) {
      notify(chrome.runtime.lastError.message);
    }
    else {
      if (localStorage.getItem('auto-fullscreen') === 'true') {
        chrome.windows.update(tab.windowId, {
          state: 'fullscreen'
        });
      }
      chrome.tabs.executeScript(tab.id, {
        file: 'action.js'
      });
    }
  });
}

chrome.commands.onCommand.addListener(function (command) {
  if (command === 'send-to-server') {
    chrome.tabs.query({
      active: true,
      currentWindow: true
    }, tabs => {
      if (tabs.length) {
        onClicked(tabs[0]);
      }
    });
  }
});
