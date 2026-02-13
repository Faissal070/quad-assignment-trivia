# quad-assignment-trivia
This is a simple trivia quiz application built using React. It allows users to test their knowledge on various topics by answering multiple-choice questions.
The backend is responsible for providing trivia questions, while the frontend displays these questions as multiple-choice options to the user.

A key design goal of this project was to prevent users from being able to determine the correct answers via browser developer tools or other client-side inspection methods.
For this reason, the API only exposes the data required to display the questions and answer options, while validation of the selected answer is handled server-side.

## Features
- Multiple-choice trivia questions
- ASP.NET Web API for retrieving questions
- Server-side answer validation
- Clear separation between frontend and backend
- Responsive frontend design

## Technologies Used
- ASP.NET Web API
- React
- TailWind CSS

# Clone the repository
```bash
https://github.com/Faissal070/quad-assignment-trivia.git
```

# How to use locally:

# backend
To start the backend, run the Trivia.Api project.
Once the API is running, navigate to:
https://localhost:7035/swagger/index.html

The Swagger UI exposes two endpoints that can be used manually:

GET endpoint
- This endpoint is used to create or retrieve a quiz.
- You provide a quizId (GUID) and the total number of questions to retrieve the quiz questions.

POST endpoint
- This endpoint is used to validate answers.
- By providing the questionId, quizId, and the selected answer, the API verifies whether the answer is correct.

# Front-end
To start the frontend, install the dependencies and run the development server:
```bash
- npm install
- npm run dev
```

Copy the URL, open it in your browser, and the questions will be fetched so the quiz can be started.
