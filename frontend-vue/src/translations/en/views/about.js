export default {
  header: "Description of test assignment (original)",
  p1: "The task is to implement a simple web application that allows to manage the requests sent to the support. The functionality of the simplified system would be as follows.",
  list1: [
    "The user can enter a request,",
    "The request must contain a description, creation time, the deadline time for resolution; the creation time must be timestamp by system, the other mandatory fields are filled in by the user,",
    "The user is shown active requests with all fields sorted in descending order by resolution deadline.",
    "Requests that are less than 1 hour away or have already expired will be listed in red in the list.",
    "The user can mark requests resolved in the list, which removes it from the list.",
  ],
  p2: "A modern browser (HTML5, etc.) can be assumed. It is not necessary to store the data in the database, you can freely keep it in the memory as well. We would definitely like to see unit tests at work as well.",
};