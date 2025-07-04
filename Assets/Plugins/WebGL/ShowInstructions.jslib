mergeInto(LibraryManager.library, {
  ShowInstructions: function () {
    if (typeof ShowInstructions === "function") {
      ShowInstructions();
    }
  }
});
