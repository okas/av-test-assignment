export default {
  section: [
    {
      header: "Descripción de la obra de prueba (original)",
      p1: "La tarea es implementar una aplicación web sencilla que permita gestionar las peticiones enviadas al soporte. La funcionalidad del sistema simplificado sería la siguiente.",
      list1: [
        "El usuario puede entrar una petición.",
        "La petición debe contener una descripción, hora de creación, la hora límite para la resolución; la hora de creación debe ser fechada por el sistema, los demás campos obligatorios son rellenados por el usuario.",
        "Al usuario se le muestran las solicitudes activas con todos los campos ordenados en orden descendente por fecha límite de resolución.",
        "Las solicitudes que están a menos de 1 hora de plazo que ya hayan vencido aparecerán en rojo en la lista.",
        "El usuario puede marcar solicitudes resueltas en la lista, lo que lo elimina de la lista.",
      ],
      p2: "Se puede suponer un navegador moderno (HTML5, etc.). No es necesario almacenar los datos en la base de datos, también puede guardarlos libremente en la memoria. Definitivamente nos gustaría ver pruebas unitarias en funcionamiento también.",
    },
    {
      header: "Funcionalidades extras",
      p1: "Decidí desarrollar más este proyecto para experimentar las funciones de ASP.NET Core Web API, VueJS y otras cosas relacionadas con estos.",
      p2: "Por 02. febrero 2022:",
      list1: [
        "Los partes de Vue se han refactorizado a la API de composición.",
        "Experimentación de traducción de textos, soporte multilingüe. Complemento escrito para cargar dinámicamente textos en diferentes idiomas para todas las vistas o componentes, que tienen cualquier tipo de texto visible.",
      ],
    },
  ],
};
