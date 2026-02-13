import './App.css'
import { useEffect, useState } from 'react'

function App() {
    useEffect(() => {
        startNewQuiz()
    }, [])

    const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

    const [quizId, setQuizId] = useState(null)
    const [questions, setQuestions] = useState([])
    const [selectedAnswers, setSelectedAnswers] = useState({})
    const [answerResults, setAnswerResults] = useState({})
    const [isSubmitted, setIsSubmitted] = useState(false)

    const allAnswered =
        questions.length > 0 &&
        Object.keys(selectedAnswers).length === questions.length

    const startNewQuiz = () => {
        setQuestions([])
        setSelectedAnswers({})
        setAnswerResults({})
        setIsSubmitted(false)

        const id = createQuizId()
        fetchQuestions(id)
    }

    const createQuizId = () => {
        const id = crypto.randomUUID()
        setQuizId(id)
        return id
    }

    const fetchQuestions = async (quizId) => {
        if (!API_BASE_URL) return('Vite base url is not set')   

        try {
            const response = await fetch(
                `${API_BASE_URL}/questions?amount=10&quizId=${quizId}`
            )

            if (!response.ok) {
                throw new Error(data.message || 'Failed to fetch questions')
            }

            const data = await response.json()
            setQuestions(data.data)
        } catch (error) {
            console.error('Fetch failed:', error)
        }
    }

    /*Handling answers*/
    const handleAnswerSelect = (questionId, answerIndex) => {
        if (isSubmitted) return

        setSelectedAnswers(prev => ({
            ...prev,
            [questionId]: answerIndex,
        }))
    }

    const buildSubmitPayload = () => ({
        quizId,
        answers: Object.entries(selectedAnswers).map(
            ([questionId, optionIndex]) => {
                const question = questions.find(q => q.id === questionId)

                return {
                    questionId,
                    selectedAnswer: question.choices[optionIndex],
                }
            }
        ),
    })

    const handleSubmit = async () => {
        try {
            const response = await fetch(`${API_BASE_URL}/answers`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(buildSubmitPayload()),
            })

            const result = await response.json()

            if (!response.ok) {
                throw new Error(result.message)
            }

            const resultsMap = {}
            result.data.forEach(r => {
                resultsMap[r.questionId] = r.isCorrect
            })

            setAnswerResults(resultsMap)
            setIsSubmitted(true)
        } catch (error) {
            console.error(error.message)
        }
    }

    /*       UI*/
    return (
        <div className="flex justify-center min-h-screen bg-gray-100 py-8">
            <div className="w-full max-w-lg bg-white p-5 rounded shadow-lg">

                <h1 className="p-2 border rounded text-center font-bold mb-6 text-xl">
                    Quiz
                </h1>

                {questions.map(question => (
                    <div key={question.id} className="mb-8">
                        <div className="font-semibold mb-2">
                            {question.question}
                        </div>

                        {question.choices.map((choice, index) => {
                            const isSelected =
                                selectedAnswers[question.id] === index
                            const isCorrect =
                                answerResults[question.id]

                            let style =
                                'border-gray-300 hover:bg-gray-50'

                            if (!isSubmitted && isSelected) {
                                style =
                                    'border-gray-500 bg-yellow-50'
                            }

                            if (isSubmitted && isSelected) {
                                style = isCorrect
                                    ? 'border-green-500 bg-green-50'
                                    : 'border-red-500 bg-red-50'
                            }

                            return (
                                <button
                                    key={index}
                                    onClick={() =>
                                        handleAnswerSelect(
                                            question.id,
                                            index
                                        )
                                    }
                                    className={`block w-full p-2 mt-2 rounded border transition ${style}`}
                                >
                                    {choice}
                                </button>
                            )
                        })}
                    </div>
                ))}

                {!isSubmitted && (
                    <button
                        onClick={handleSubmit}
                        disabled={!allAnswered}
                        className={`w-full mt-4 p-3 rounded font-semibold transition
                            ${allAnswered
                                ? 'bg-blue-600 text-white hover:bg-blue-700'
                                : 'bg-gray-300 text-gray-500 cursor-not-allowed'
                            }`}
                    >
                        Submit answers
                    </button>
                )}

                {isSubmitted && (
                    <button
                        onClick={startNewQuiz}
                        className="w-full mt-4 bg-green-600 text-white p-3 rounded font-semibold hover:bg-green-700 transition"
                    >
                        Next questions
                    </button>
                )}
            </div>
        </div>
    )
}

export default App
