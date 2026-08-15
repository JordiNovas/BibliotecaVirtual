# 📚 Sistema de Biblioteca Virtual (`BibliotecaVirtual.Web`)

![.NET Core](https://img.shields.io/badge/.NET%20Core-8.0-512BD4?style=for-the-badge&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp)
![EF Core](https://img.shields.io/badge/Entity%20Framework-512BD4?style=for-the-badge&logo=dotnet)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC292B?style=for-the-badge&logo=microsoft-sql-server)
![Selenium](https://img.shields.io/badge/Selenium-43B02A?style=for-the-badge&logo=selenium)
![xUnit](https://img.shields.io/badge/xUnit-512BD4?style=for-the-badge)

Aplicación web desarrollada en **ASP.NET Core MVC** para la gestión integral de una biblioteca virtual. El sistema permite administrar usuarios, explorar un catálogo de libros interactivo, procesar préstamos/devoluciones en tiempo real y cuenta con una suite completa de pruebas unitarias y automatizadas de UI.

---

## 👨‍💻 Datos del Autor

* **Estudiante:** Jordi Alexander Novas Franco
* **Matrícula:** 20231205
* **Institución:** Instituto Tecnológico de Las Américas (ITLA)

---

## 🚀 Características Principales

* **🔒 Autenticación y Autorización:** Control de acceso seguro mediante cookies de sesión y roles (`[Authorize]`).
* **👥 Gestión Administrativa de Usuarios:** CRUD completo para la administración de cuentas de usuario.
* **📖 Catálogo de Libros Interactivo:** Consulta de disponibilidad de ejemplares con actualización dinámicas de estado.
* **🔄 Ciclo de Préstamos y Devoluciones:** Gestión de reservas vinculadas al perfil de usuario activo y registro de fechas de devolución.
* **🧪 Suite de Pruebas Automatizadas:** 
  * Pruebas Unitarias con **xUnit**.
  * Pruebas de Interfaz (E2E) con **Selenium WebDriver** implementando el patrón **Page Object Model (POM)**.

---

## 🛠️ Tecnologías Utilizadas

* **Backend:** C#, ASP.NET Core MVC, Entity Framework Core
* **Base de Datos:** SQL Server
* **Frontend:** Razor Views, HTML5, CSS3, Bootstrap 5
* **Testing & QA:** xUnit, Selenium WebDriver (Chrome Driver)
* **Gestión de Proyecto:** Git, GitHub, Atlassian Jira (Scrum)

---

## 📋 Estructura del Proyecto

```text
BibliotecaVirtual/
├── BibliotecaVirtual.Web/          # Proyecto Principal (MVC)
│   ├── Controllers/               # Controladores (Cuenta, Usuarios, Libros)
│   ├── Data/                      # DbContext y Migraciones (EF Core)
│   ├── Models/                    # Entidades del Dominio (Usuario, Libro, Prestamo)
│   └── Views/                     # Vistas Razor e Interfaz Bootstrap
│
└── BibliotecaVirtual.Tests/        # Proyecto de Pruebas (xUnit & Selenium)
    ├── PageObjects/               # Clases del Patrón Page Object Model (POM)
    └── Tests/                     # Suite de Pruebas Unitarias y UI
