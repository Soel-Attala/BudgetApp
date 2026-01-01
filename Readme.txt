# 🧾 Presupuestos

**Presupuestos** es una aplicación de escritorio desarrollada en **.NET 9 (WPF + SQLite)** que permite gestionar materiales, mano de obra y presupuestos de forma ágil y profesional.  
Está pensada para ingenieros, arquitectos y profesionales de la construcción que necesiten generar cotizaciones rápidas y precisas.

---

## 🚀 Tecnologías utilizadas
- **.NET 9 / WPF** → Interfaz moderna con Material Design.
- **SQLite** → Base de datos local, portable y sin necesidad de servidor externo.
- **Entity Framework Core** → Acceso a datos con migraciones automáticas.
- **CommunityToolkit.MVVM** → Patrón MVVM simplificado.
- **QuestPDF** → Exportación de presupuestos a PDF.
- **MaterialDesignInXAML** → UI estilizada y responsiva.

---

## ✨ Funcionalidades principales
- 📦 **Gestión de materiales** en ARS y USD (con conversión automática por valor del dólar).
- 👷 **Gestión de mano de obra**, reutilizable en distintos presupuestos.
- 🗂 **Subgrupos de materiales** (estructuras predefinidas).
- 💰 Cálculo automático de **honorarios y beneficios**.
- 🧾 Exportación de presupuestos en **formato PDF**.
- 🖥 Instalador MSI para Windows (sin necesidad de configurar nada).

---

## 📋 Requisitos del sistema
- **Sistema operativo**: Windows 10/11 (x64).
- **.NET 9 Desktop Runtime** instalado.
- **RAM**: 2 GB mínimo.
- **Almacenamiento**: 200 MB libres.

---

## 🔧 Instalación
1. Ejecutar el archivo `Presupuestos_Setup.msi`.
2. Seguir las instrucciones del asistente.
3. Abrir el acceso directo desde el menú Inicio o Escritorio.

---

## 📜 Licencia
Este proyecto se distribuye bajo la licencia **MIT**.  
Podés usarlo, modificarlo y compartirlo libremente, siempre manteniendo los créditos.

---

## 👨‍💻 Autor
**Attala Ingeniería**  
📧 contacto@attalaingenieria.com  
🌐 [GitHub Repository](https://github.com/Soel-Attala/BudgetApp)






English Version

# 🧾 Budget Management System

**Budget Management System** is a desktop application built with **.NET 9 (WPF + SQLite)** that enables efficient and professional management of materials, labor costs, and project budgets.  
Designed for engineers, architects, and construction professionals who need to generate accurate quotations quickly.

---

## 🚀 Technologies

- **.NET 9 / WPF** → Modern UI with Material Design
- **SQLite** → Portable, local database with no external server required
- **Entity Framework Core** → Data access with automatic migrations
- **CommunityToolkit.MVVM** → Simplified MVVM pattern implementation
- **QuestPDF** → Professional PDF export functionality
- **MaterialDesignInXAML** → Styled and responsive user interface

---

## ✨ Key Features

- 📦 **Material Management** in ARS and USD (with automatic currency conversion)
- 👷 **Labor Cost Management**, reusable across multiple budgets
- 🗂 **Material Subgroups** with predefined structures
- 💰 Automatic calculation of **fees and profit margins**
- 🧾 Professional **PDF budget export**
- 🖥 MSI installer for Windows (zero configuration required)
- 🔄 Real-time currency conversion
- 📊 Comprehensive budget breakdown and reporting

---

## 🏗️ Architecture

The application follows the **MVVM (Model-View-ViewModel)** pattern with clear separation of concerns:
```
Presupuestos/
├── Models/          # Domain entities and business logic
├── ViewModels/      # Presentation logic and data binding
├── Views/           # XAML user interface
├── Services/        # Business services (PDF, calculations, etc.)
├── Data/            # Entity Framework DbContext and migrations
└── Resources/       # Styles, templates, and localization
```

**Design Patterns Used:**
- **MVVM** for UI separation
- **Repository Pattern** for data access
- **Dependency Injection** for loose coupling
- **Command Pattern** for user actions

---

## 📋 System Requirements

- **Operating System**: Windows 10/11 (x64)
- **.NET 9 Desktop Runtime** installed ([Download here](https://dotnet.microsoft.com/download/dotnet/9.0))
- **RAM**: 2 GB minimum
- **Storage**: 200 MB available space

---

## 🔧 Installation

### Option 1: MSI Installer (Recommended)
1. Download `Presupuestos_Setup.msi`
2. Run the installer and follow the wizard
3. Launch from Start Menu or Desktop shortcut

### Option 2: Build from Source
```bash
# Clone the repository
git clone https://github.com/Soel-Attala/BudgetApp.git
cd BudgetApp/Presupuestos

# Restore dependencies
dotnet restore

# Build the project
dotnet build --configuration Release

# Run the application
dotnet run
```

---

## 💻 Usage

### Creating a New Budget
1. Navigate to **Materials** section
2. Add materials with prices in ARS or USD
3. Go to **Labor** section and define labor costs
4. Create a new **Budget** and select materials/labor
5. System automatically calculates totals with fees
6. Export to PDF for professional presentation

### Currency Management
- Set current USD exchange rate in Settings
- Materials can be entered in either ARS or USD
- Automatic conversion displays both currencies
- Historical rates tracking (optional)

### PDF Export
- Professional formatted documents
- Company logo and details
- Itemized breakdown
- Subtotals, taxes, and grand total
- Client information section

---

## 🗄️ Database Schema

The SQLite database includes the following main entities:

- **Materials**: Product catalog with pricing
- **Labor**: Workforce cost definitions
- **Budgets**: Complete project quotations
- **BudgetItems**: Line items linking budgets to materials/labor
- **Subgroups**: Categorization for materials
- **Settings**: Application configuration (exchange rate, fees, etc.)

---

## 🛠️ Development

### Prerequisites
```bash
# Install .NET 9 SDK
winget install Microsoft.DotNet.SDK.9

# Recommended IDE
- Visual Studio 2022 (Community or higher)
- JetBrains Rider
```

### Running Locally
```bash
# Navigate to project directory
cd Presupuestos

# Apply database migrations
dotnet ef database update

# Run the application
dotnet run
```

### Building Installer
```bash
# Build release version
dotnet publish -c Release -r win-x64 --self-contained false

# Use WiX Toolset to create MSI
# (See build scripts in /installer directory)
```

---

## 🧪 Testing
```bash
# Run unit tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

---

## 📝 Code Standards

This project follows:
- **C# Coding Conventions** from Microsoft
- **MVVM Best Practices** for WPF applications
- **SOLID Principles** for maintainable code
- **XML Documentation** for public APIs
- **Async/Await** for responsive UI

---

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'feat: add amazing feature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

Please follow [Conventional Commits](https://www.conventionalcommits.org/) for commit messages.

---

## 📜 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

You are free to use, modify, and distribute this software, provided you maintain the original copyright notice.

---

## 👨‍💻 Author

**Soel Attala**  
📧 [soelattala.dev@gmail.com](mailto:soelattala.dev@gmail.com)  
🌐 [GitHub](https://github.com/Soel-Attala)  
💼 [LinkedIn](https://www.linkedin.com/in/soel-attala)


