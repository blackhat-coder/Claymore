![Screenshot](/Screenshot.png)

<center><h1>Claymore</h1></center>

Claymore is an open-source **load testing tool** designed to simulate millions of users interacting with your system.  
By defining **user behavior** in a simple JSON format, you can swarm your APIs with realistic, concurrent traffic.

---

## ✨ Features

- **JSON-based configuration**: Define tasks and user flows in a declarative way.
- **Templated payloads and headers**: Dynamically generate request data at scale.
- **Dependency-aware execution**: Chain tasks together and pass data between them.
- **Massive concurrency**: Simulate millions of simultaneous users with lightweight workers.
- **Open-source and extensible**: Fully customizable to suit any API testing scenario.

---

## 🚀 Getting Started

### Installation and Quick Start

1.  **Download and Extract:**
    Download the latest release of Claymore from the [releases page](https://github.com/blackhat-coder/Claymore) and extract the contents to a folder of your choice.

2.  **Run the Program:**
    Open your command prompt or terminal, navigate to the folder where you extracted the files, and run the program using one of the following methods:

    * **Specify a Configuration File:**
        Provide the path to your JSON [Configuration](#-configuration) file as a command-line argument. This is ideal for scripting or automated runs.
        ```
        .\Claymore.exe <config_filepath>
        ```
        **Example:**
        ```
        .\Claymore.exe C:\Users\YourName\Documents\test-config.json
        ```

    * **Interactive Mode:**
        If you don't provide a configuration file, Claymore will prompt you to enter the path. This is useful for first-time use or manual runs.
        ```
        .\Claymore.exe
        ```
        *The program will then ask you to enter the filepath.*


### Build
Clone the repository:

```bash
# Clone the repository
git clone https://github.com/blackhat-coder/Claymore.git

# Navigate into the source folder
cd Claymore

# Build the project
dotnet build
```


## 🔧 Configuration

Claymore uses a single JSON file to define all the tasks (user actions) and their relationships.

Here's an example configuration:

```json
{
    "tasks": [
        {
            "name":"fetchProfile",
            "workers":100,
            "endpoint":"https://api.example.com/profile",
            "order":1,
            "method":"POST",
            "headers":[],
            "payload":"",
            "dependsOn":[]
        }
    ]
}
```

---

### Task Properties

| Property      | Description |
| :---        |    :----:   |
| `name`      | Unique identifier for the task. Used for templating and dependencies.       |
| `workers`  | Number of concurrent users (workers) to run this task.        |
|`endpoint`| The URL that the workers will call.
| `order` | Execution order. Lower numbers run first.
| `method` | HTTP method (`GET`, `POST`, `PUT`, `DELETE`).
| `headers` | list of HTTP headers (key-value pairs). Supports templating.
| `payload` | HTTP request body. Supports templating.
| `dependsOn` | List of other task names this task depends on (for chaining and data passing).


### 🛠️ Templating

Claymore supports **placeholders** in `headers` and `payload` that are replaced at runtime, enabling unique, dynamic data for every simulated user.

**Available Placeholders**

| Placeholder      | Description |
| :---        |    :----:   |
| `$name.ResponseBody.property`      | Fetches `property` from the **response body** of the task named `name`.
| `$name.ResponseHeader.property` | Fetches `property` from the **response header** of the task named `name`.
| `$bool` | Replaces with random boolean value (`true` or `false`).
| `$string[6]` | Replaces with a random string of length `6`.
| `$name` | Replaces with a random name.
| `$number[5]` | Replaces with a random numeric string of length `5`.
| `$email` | Replaces with a random email.

## 🛠️ Templating Example

Here’s an example configuration that uses templating to dynamically generate request data and pass values between tasks:

```json
{
  "tasks": [
    {
      "name": "login",
      "workers": 500,
      "endpoint": "https://api.example.com/login",
      "order": 1,
      "method": "POST",
      "headers": [
        { "Content-Type": "application/json" }
      ],
      "payload": "{\"username\": \"$name\", \"password\": \"$string[8]\"}",
      "dependsOn": []
    },
    {
      "name": "fetchProfile",
      "workers": 500,
      "endpoint": "https://api.example.com/profile",
      "order": 2,
      "method": "GET",
      "headers": [
        { "Authorization": "Bearer $login.ResponseBody.token" }
      ],
      "payload": "",
      "dependsOn": ["login"]
    }
  ]
}
```

### Use Cases
- Stress testing APIs at scale.
- Benchmarking backend performance.
- Testing microservicves dependencies and latency
- Simulating realistic traffic patterns before production releases

---

### 🫱🏽‍🫲🏾 Contributing
We welcome contributions! If you have an idea for a new feature, a bug fix, or an improvement, please feel free to:

1. Open an issue on the [Github issues page](https://github.com/blackhat-coder/Claymore/issues) to discuss your idea.
2. Fork the repository and create a new branch for your changes.
3. Submit a merge request.

### ⚖️Licesing

Claymore is released under the MIT License. This is a permissive open-source license that allows you to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the software. For the full license text, please see the [LICENSE](https://github.com/blackhat-coder/Claymore/blob/master/LICENSE) file in the project repository.