# 🧠 Personality Test Website

A comprehensive personality assessment application built with React frontend and .NET Core Web API backend, implementing the Big Five personality model.

## ✨ Features

- **📊 Interactive Personality Test**: Complete Big Five personality assessment
- **⚡ Real-time Progress**: Dynamic question flow with live progress tracking
- **📈 Detailed Analytics**: Comprehensive personality reports with insights
- **🌐 Arabic Interface**: RTL Arabic UI with responsive design
- **🔌 RESTful API**: Clean and scalable backend architecture
- **📱 Mobile Friendly**: Responsive design for all devices

## 🛠️ Technology Stack

### Frontend
- **React 18.2.0**: Modern UI framework
- **HTML5 & CSS3**: Responsive design with RTL support
- **JavaScript ES6+**: Interactive user experience

### Backend
- **.NET Core 9.0**: High-performance Web API
- **Entity Framework Core**: Database ORM
- **SQL Server**: Relational database (configurable)
- **Swagger**: API documentation

### Development Tools
- **Visual Studio Code**: Primary IDE
- **Git**: Version control
- **npm**: Package management
- **dotnet CLI**: .NET development tools

## 🏗️ Project Architecture

```
📁 Root/
├── 🎨 src/                           # React Frontend
│   ├── App.js                        # Main React component
│   ├── App.css                       # RTL Arabic styles
│   └── index.js                      # Application entry point
├── 🔧 PersonalityTestAPI/            # .NET Core Backend
│   ├── 🎛️ Controllers/               # API endpoints
│   │   ├── SessionsController.cs     # Session management
│   │   └── AnswersController.cs      # Answer processing
│   ├── 📊 Models/                    # Data models
│   │   └── PersonalityModels.cs      # Database entities
│   ├── 🔄 Services/                  # Business logic
│   │   ├── IPersonalityTestService.cs
│   │   └── PersonalityTestService.cs
│   ├── 📤 DTOs/                      # Data transfer objects
│   │   └── PersonalityDTOs.cs
│   ├── 🗄️ Data/                      # Database context
│   │   └── PersonalityTestContext.cs
│   └── ⚙️ Program.cs                 # App configuration
├── 📱 public/                        # Static assets
│   └── index.html                    # HTML template
├── 📦 package.json                   # npm dependencies
└── 📚 README.md                      # Documentation
```

## � Quick Start

### 📋 Prerequisites
- **Node.js** v16+ 
- **.NET 9.0 SDK**
- **Git**

### 🛠️ Installation & Setup

1. **📥 Clone the repository**
   ```bash
   git clone https://github.com/AmiraSayedMohamed/PersonalityTestWebsite-FrontReact-Backend.NET-Core-Web-API.git
   cd PersonalityTestWebsite-FrontReact-Backend.NET-Core-Web-API
   ```

2. **🎯 Setup Frontend (React)**
   ```bash
   npm install
   npm start
   ```
   🌐 Frontend runs on: `http://localhost:3000`

3. **⚙️ Setup Backend (.NET Core)**
   ```bash
   cd PersonalityTestAPI
   dotnet restore
   dotnet run
   ```
   🔗 Backend API runs on: `http://localhost:5000`

4. **🎉 Open your browser**
   Navigate to `http://localhost:3000` and start the personality test!

## 🌍 Screenshots & Demo

### 🏠 Home Screen
![Home Screen](https://via.placeholder.com/800x400/4f46e5/ffffff?text=Arabic+RTL+Interface)

### 📝 Question Interface  
![Question Screen](https://via.placeholder.com/800x400/059669/ffffff?text=Interactive+Questions)

### 📊 Results Dashboard
![Results Screen](https://via.placeholder.com/800x400/dc2626/ffffff?text=Personality+Analysis)

## 🎯 Big Five Personality Dimensions

| 🔍 Dimension | 📖 Description |
|--------------|----------------|
| **🌟 Openness** | Creativity, curiosity, and openness to new experiences |
| **⚡ Conscientiousness** | Organization, discipline, and goal-directed behavior |
| **🤝 Extraversion** | Sociability, assertiveness, and positive emotions |
| **💝 Agreeableness** | Cooperation, trust, and empathy towards others |
| **😰 Neuroticism** | Emotional stability and stress management |

## 📊 API Endpoints

### Sessions
- `POST /api/sessions` - Create new test session
- `GET /api/sessions/{id}/question` - Get current question
- `GET /api/sessions/{id}/report` - Get personality report

### Answers
- `POST /api/answers` - Submit answer

## 🚀 Technical Features

### 🎨 Frontend (React)
- ⚡ **Modern React 18.2.0** with Hooks
- 🌐 **RTL Arabic Interface** with proper localization
- 📱 **Responsive Design** for all screen sizes
- 🎭 **Interactive UI** with smooth transitions
- ✅ **Form Validation** and error handling

### 🔧 Backend (.NET Core)
- 🏗️ **.NET 9.0 Web API** with clean architecture
- 🗄️ **Entity Framework Core** with InMemory database
- 📡 **RESTful APIs** following best practices
- 🔐 **CORS Configuration** for cross-origin requests
- 📖 **Swagger Documentation** for API testing
- 🏷️ **Data Transfer Objects** for clean data flow

### 🎯 Core Features
- 📊 **Big Five Model Implementation**
- 🔄 **Dynamic Question Flow**
- 📈 **Real-time Progress Tracking**  
- 🧮 **Advanced Scoring Algorithm**
- 📋 **Comprehensive Reports**

## 🌍 Internationalization
- **RTL Support**: Right-to-left layout for Arabic interface
- **Arabic UI**: Native Arabic user interface
- **Localized Content**: Culture-appropriate content

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👨‍💻 Author

**Amira Sayed Mohamed**
- GitHub: [@AmiraSayedMohamed](https://github.com/AmiraSayedMohamed)

## 🙏 Acknowledgments

- Big Five personality model research
- React.js community
- .NET Core documentation
- Open source contributors

---

⭐ **Star this repository if you found it helpful!**
