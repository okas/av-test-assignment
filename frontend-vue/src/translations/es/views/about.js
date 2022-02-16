export default {
  section: [
    {
      header: {
        h2: "Descripción de la obra de prueba (original)",
      },
      p_1: "La tarea es implementar una aplicación web sencilla que permita gestionar las peticiones enviadas al soporte. La funcionalidad del sistema simplificado sería la siguiente.",
      ol: {
        li: [
          "El usuario puede entrar una petición.",
          "La petición debe contener una descripción, hora de creación, la hora límite para la resolución; la hora de creación debe ser fechada por el sistema, los demás campos obligatorios son rellenados por el usuario.",
          "Al usuario se le muestran las solicitudes activas con todos los campos ordenados en orden descendente por fecha límite de resolución.",
          "Las solicitudes que están a menos de 1 hora de plazo que ya hayan vencido aparecerán en rojo en la lista.",
          "El usuario puede marcar solicitudes resueltas en la lista, lo que lo elimina de la lista.",
        ],
      },
      p_2: "Se puede suponer un navegador moderno (HTML5, etc.). No es necesario almacenar los datos en la base de datos, también puede guardarlos libremente en la memoria. Definitivamente nos gustaría ver pruebas unitarias en funcionamiento también.",
    },
    {
      header: {
        h2: "Funcionalidades extras",
      },
      p_1: "Decidí desarrollar más este proyecto para experimentar las funciones de ASP.NET Core Web API, VueJS y otras cosas relacionadas con estos.",
      section: [
        {
          header: {
            h3: "Por 02. febrero 2022:",
          },
          ol: {
            li: [
              "Los partes de Vue se han refactorizado a la API de composición.",
              "Experimentación de traducción de textos, soporte multilingüe. Complemento escrito para cargar dinámicamente textos en diferentes idiomas para todas las vistas o componentes, que tienen cualquier tipo de texto visible.",
            ],
          },
        },
        {
          header: {
            h3: "Por 16. febrero 2022:",
          },
          h4_1: "Cambios de manejo de idioma:",
          ol_1: {
            li: [
              "En la primera visita a la aplicación, el idioma principal de los navegadores se analiza con los idiomas admitidos. Si hay una coincidencia, el idioma del navegador se establece en el idioma de la aplicación; de lo contrario, vuelve al idioma predeterminado.",
              "En el cambio de idioma, se establece <html lang>.",
              "En el cambio de idioma, se almacena en el objeto `window.localStorage`.",
              "Al cambiar el idioma, se establece el título del documento.",
              "El cambio de idioma también actualizará los formatos de fecha y hora en los datos originados por la API, sin solicitarlo a la API.",
            ],
          },
          h4_2: "Vista 'Información' (actual) usa nuevo componente `Tree`:",
          ol_2: {
            li: [
              "Tree lee los datos estructurados HTML de la estructura JSON y los presenta a la vista.",
              "Estos datos de origen deben contener una estructura HTML, donde las propiedades son etiquetas HTML.",
              "Se admiten Array, Object y String tipos de Javascript.",
            ],
          },
        },
      ],
    },
  ],
};
