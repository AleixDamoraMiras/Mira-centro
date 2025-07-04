mergeInto(LibraryManager.library, {
  NotifyGameEnded: function(userIdPtr) {
    const userId = UTF8ToString(userIdPtr);

    if (typeof onGameEnded === "function") {
      onGameEnded(userId);
    }
  }
});
