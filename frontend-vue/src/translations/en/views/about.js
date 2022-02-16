export default {
  section: [
    {
      header: {
        h2: "Description of test assignment (original)",
      },
      p_1: "The task is to implement a simple web application that allows to manage the requests sent to the support. The functionality of the simplified system would be as follows.",
      ol: {
        li: [
          "The user can enter a request.",
          "The request must contain a description, creation time, the deadline time for resolution; the creation time must be timestamp by system, the other mandatory fields are filled in by the user.",
          "The user is shown active requests with all fields sorted in descending order by resolution deadline.",
          "Requests that are less than 1 hour away or have already expired will be listed in red in the list.",
          "The user can mark requests resolved in the list, which removes it from the list.",
        ],
      },
      p_2: "A modern browser (HTML5, etc.) can be assumed. It is not necessary to store the data in the database, you can freely keep it in the memory as well. We would definitely like to see unit tests at work as well.",
    },
    {
      header: {
        h2: "Added functionalities",
      },
      p_1: "I decided to develop further on this project to experiment features of ASP.NET Core Web API, VueJS and other things that are related to these.",
      section: [
        {
          header: {
            h3: "By February 02, 2022:",
          },
          ol: {
            li: [
              "Vue bits have been refactored to Composition API.",
              "Experimenting texts translation, multi language support. Wrote plugin to dynamically load texts in different languages for all the views or components, that have any kind of visible texts.",
            ],
          },
        },
        {
          header: {
            h3: "By February 16, 2022:",
          },
          h4_1: "Changes regarding language management:",
          ol_1: {
            li: [
              "On the first visit to app, browsers main language is analyzed against supported languages. If there is a match, then browser language is set to app language, otherwise falls back to default language.",
              "On language change <html lang> is set.",
              "On language change it is stored into `window.localStorage` object.",
              "On language change document title is set.",
              "Language change will update date and time formats on API originated data as well, without requesting it from API.",
            ],
          },
          h4_2: "View 'About' (current) uses new `Tree` component:",
          ol_2: {
            li: [
              "Tree reads the HTML structured data from JSON structure and renders it into view.",
              "This source data must contain HTML structure, where properties are HTML tags.",
              "Javascript Arrays, Objects and String values are supported.",
            ],
          },
        },
      ],
    },
  ],
};
