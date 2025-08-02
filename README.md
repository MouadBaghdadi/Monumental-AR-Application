# AR Virtual Museum - MONUMONTAL - Seven Wonders of the World

## Project Description
This project is an Augmented Reality (AR) mobile application developed for a university assignment. It transforms any space into an interactive virtual museum showcasing 3D models of the Seven Wonders of the Ancient World (and other notable world wonders). Users can explore these wonders, view information, listen to audio descriptions, and take interactive quizzes.

This application is built using Unity3D and Vuforia, with scripting in C#.

## Features
*   **Augmented Reality Experience**: Uses image targets or surface detection to place virtual content in the real world.
*   **3D Models of World Wonders**: Includes detailed 3D models of:
    *   Great Pyramid of Giza
    *   Great Wall China
    *   Colosseum
    *   CHichenItza
    *   TajMahal
    *   The Great Sphinx
*   **Interactive Elements**:
    *   Buttons for navigation and interaction.
    *   Floating text panels displaying information about each wonder.
    *   Audio narrations for a guided experience.
*   **Multilingual Support**: Information and UI available in English, French, and Arabic.
*   **Interactive Quiz**: Test user knowledge about the wonders with multiple-choice questions, available in all supported languages.
*   **Object Scaling**: Users can scale the 3D models using pinch gestures.
*   **Day/Night Mode**: Visual toggle for different lighting conditions.

## Technologies Used
*   **Game Engine**: Unity 2019.4.1f1
*   **AR SDK**: Vuforia Engine
*   **Programming Language**: C#
*   **3D Modeling**: I used Free Models
*   **UI**: Unity UI, TextMeshPro
*   **Libraries**:
    *   LeanTouch (for touch input like scaling)
    *   ArabicSupport (for proper Arabic text rendering)

## Setup and Installation
1.  **Clone the repository:**
    ```sh
    git clone https://github.com/your-username/your-repository-name.git
    ```
2.  **Open in Unity Hub:**
    *   Open Unity Hub.
    *   Click "Add" or "Open" and navigate to the cloned project folder.
    *   Select the correct Unity Editor version (as specified above or the one the project was last opened with).
3.  **Vuforia Setup:**
    *   Ensure Vuforia Engine is enabled in `Project Settings > XR Plug-in Management`.
    *   If using image targets, make sure they are configured in the Vuforia Developer Portal and imported into the project (`Window > Vuforia Configuration`).
4.  **Import Required Assets (if not included or using Asset Store versions):**
    *   TextMeshPro: Usually imported via `Window > TextMeshPro > Import TMP Essential Resources`.
    *   LeanTouch: If not already in `Assets/Lean`, import from the Unity Asset Store.
    *   ArabicSupport: If not already in `Assets/`, import from its source.
5.  **Build for Target Platform (Android/iOS):**
    *   Go to `File > Build Settings`.
    *   Select your target platform (Android or iOS).
    *   Ensure all scenes (e.g., MainMenu, ARMuseum, Quiz) are added to the "Scenes In Build".
    *   Configure Player Settings (Bundle Identifier, Company Name, etc.).
    *   Click "Build" or "Build and Run".

## How to Use
1.  Launch the application on your mobile device.
2.  Point your device's camera at a designated image target.
3.  Navigate through the available wonders using UI buttons.
4.  Tap on information icons to view details and listen to audio.
5.  Use pinch gestures to scale the 3D models.
6.  Access the quiz section for a selected wonder and answer questions.
7.  Switch languages using the language selection button.

## Project Structure
*   **`Assets/Scenes/`**: Contains all Unity scenes (e.g., `MainMenu.unity`, `ARMuseum.unity`, `Quiz.unity`).
*   **`Assets/Scripts/`**: Contains all C# scripts.
    *   `AR/`: Scripts related to AR functionality (e.g., placement, scaling).
    *   `Exhibits/`: Scripts for managing exhibit data and interactions.
    *   `UI/`: Scripts for UI management and interactions.
    *   `Quiz/`: Scripts for the quiz system (e.g., [`QuizManager.cs`](Assets/script/QuizManager.cs)).
*   **`Assets/Models/`**: 3D models of the world wonders.
*   **`Assets/Materials/`**: Materials used for models and UI.
*   **`Assets/Prefabs/`**: Reusable game objects, including UI elements and exhibit setups.
*   **`Assets/Sounds/`**: Audio files (music, sound effects, narrations).
*   **`Assets/TextMesh Pro/`**: Resources for TextMeshPro.
*   **`Assets/Lean/`**: Assets for the LeanTouch library.
*   **`Assets/Resources/`**: Resources loaded dynamically.
*   **`Assets/StreamingAssets/Vuforia/`**: Vuforia databases and configurations.
