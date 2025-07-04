mergeInto(LibraryManager.library, {
  SkipGame: function () {
    if (typeof SkipGame === "function") {
      SkipGame();
    }
  }
});
