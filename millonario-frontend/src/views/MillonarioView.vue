<template>
  <div>
    <div v-if="screen === 'start'" class="start-screen">
      <h1 class="title">¿Quién quiere ser millonario?</h1>

      <div v-if="isLoadingQuestions" class="loading-message">
        Cargando preguntas... por favor espera.
      </div>
      <div v-if="questionsLoadError" class="error-message">
        {{ questionsLoadError }}
      </div>

      <button class="start-button" @click="showHighScores">Ver Récords</button>
      <button
        class="start-button"
        @click="startGame"
        :disabled="isLoadingQuestions || questionsLoadError"
      >
        Comenzar Juego
      </button>
    </div>

    <div v-if="screen === 'records'" class="records-screen">
      <h1 class="title">Tabla de Récords</h1>
      <div v-if="sortedHighScores.length === 0" class="no-records">
        <p>Aún no hay récords. ¡Sé el primero en establecer uno!</p>
      </div>
      <table v-else class="high-scores-table">
        <thead>
          <tr>
            <th>Posición</th>
            <th>Jugador</th>
            <th>Premio</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(score, index) in sortedHighScores" :key="index">
            <td>{{ index + 1 }}</td>
            <td>{{ score.player }}</td>
            <td>${{ score.prize.toLocaleString() }}</td>
          </tr>
        </tbody>
      </table>
      <button class="restart-button" @click="showStartScreen">Volver al Inicio</button>
    </div>

    <div v-if="screen === 'end'" class="end-screen">
      <h1 class="title">Fin del Juego</h1>
      <div class="final-prize">Has ganado: {{ finalPrizeDisplay }}</div>
      <button class="restart-button" @click="restartGame">Jugar de nuevo</button>
      <button class="restart-button" @click="showRecordsAndSave">Guardar Récord y Ver Tabla</button>
    </div>

    <div v-if="screen === 'game'" class="game-container">
      <div class="top-info">
        <div class="timer">{{ time }}</div>
        <div class="current-prize">Premio actual: {{ currentPrizeDisplay }}</div>
      </div>
      <img
        src="https://upload.wikimedia.org/wikipedia/commons/6/61/LOGO_SPAIN_2021.jpg"
        alt="Logo Millonario"
        class="logo"
      />
      <div class="question-text">{{ currentQuestion.question }}</div>

      <div class="lifeline-container">
        <button class="lifeline" :disabled="fiftyUsed" @click="useFiftyFifty">🧠 50/50</button>
        <button class="lifeline" :disabled="audienceUsed" @click="askAudience">👥 Público</button>
        <button class="lifeline" :disabled="callUsed" @click="callFriend">📞 Amigo</button>
      </div>

      <div class="answers-grid">
        <div
          v-for="(opt, index) in currentQuestion.options"
          :key="index"
          :class="['answer', answerClass(index)]"
          @click="checkAnswer(index, opt.isCorrect)"
          v-show="visibleAnswers.includes(index)"
        >
          {{ ['A', 'B', 'C', 'D'][index] }}: <span>{{ opt.text }}</span>
        </div>
      </div>

      <div class="prize-ladder">
        <ul>
          <li
            v-for="(prize, index) in prizeValues.slice().reverse()"
            :key="index"
            :class="prizeClass(index)"
          >
            ${{ prize.toLocaleString() }}
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth' // Importa tu store de autenticación
import axios from 'axios'

const authStore = useAuthStore() // Inicializa el store

// --- Estados Reactivos ---
const QUESTIONS_API_URL = 'https://localhost:7254/api/Preguntas' // URL de tu API de preguntas

const allFetchedQuestions = ref([]) // Todas las preguntas obtenidas de la API
const shuffledQuestions = ref([]) // Preguntas seleccionadas y ordenadas para la partida actual
const current = ref(0) // Índice de la pregunta actual en shuffledQuestions

const screen = ref('start') // Controla la pantalla actual: 'start', 'game', 'end', 'records'

const time = ref(30) // Tiempo restante para responder la pregunta
let timer = null // Referencia al temporizador

const fiftyUsed = ref(false) // Indica si se usó el comodín 50/50
const audienceUsed = ref(false) // Indica si se usó el comodín del público
const callUsed = ref(false) // Indica si se usó el comodín de la llamada

const visibleAnswers = ref([0, 1, 2, 3]) // Índices de las respuestas visibles (para 50/50)

// Valores de los premios por cada nivel
const prizeValues = ref([
  100, 200, 300, 500, 1000, 2000, 4000, 8000, 16000, 32000, 64000, 125000, 250000, 500000, 1000000,
])
const safeHavenIndex = 4 // Índice del premio seguro (ej. 1000)
const finalPrize = ref(0) // Premio final ganado

const highScores = ref([]) // Array para almacenar los récords

const isLoadingQuestions = ref(false) // Estado de carga de preguntas
const questionsLoadError = ref(null) // Mensaje de error si falla la carga de preguntas

// Mapeo de niveles de juego a dificultad de preguntas
// Este mapa define qué dificultad de pregunta se requiere para cada una de las 15 preguntas del juego.
const difficultyMap = {
  0: 1, // Pregunta 1 (índice 0) requiere dificultad 1
  1: 1, // Pregunta 2 (índice 1) requiere dificultad 1
  2: 1,
  3: 1,
  4: 1, // Pregunta 5 (índice 4) requiere dificultad 1 (Última de dificultad 1)
  5: 2, // Pregunta 6 (índice 5) requiere dificultad 2
  6: 2,
  7: 2,
  8: 2,
  9: 2, // Pregunta 10 (índice 9) requiere dificultad 2 (Última de dificultad 2)
  10: 3, // Pregunta 11 (índice 10) requiere dificultad 3
  11: 3,
  12: 3,
  13: 3,
  14: 3, // Pregunta 15 (índice 14) requiere dificultad 3
}

// --- Propiedades Computadas ---
const currentUser = computed(() => {
  // console.log('authStore.user:', authStore.user) // Para depuración
  // console.log('authStore.user?.username:', authStore.user?.username) // Para depuración
  return authStore.user?.username || 'Invitado' // Obtiene el nombre de usuario o 'Invitado'
})

const sortedHighScores = computed(() => {
  // Ordena los récords de mayor a menor premio
  return [...highScores.value].sort((a, b) => b.prize - a.prize)
})

const currentQuestion = computed(() => {
  // Devuelve la pregunta actual basada en el índice 'current'
  return shuffledQuestions.value[current.value]
})

const currentPrizeDisplay = computed(() => {
  // Formatea el premio actual para mostrarlo
  if (current.value >= prizeValues.value.length) {
    return `$${prizeValues.value[prizeValues.value.length - 1].toLocaleString()}`
  }
  return `$${prizeValues.value[current.value].toLocaleString()}`
})

const finalPrizeDisplay = computed(() => {
  // Formatea el premio final para mostrarlo
  return `$${finalPrize.value.toLocaleString()}`
})

// --- Funciones de Navegación ---
function showStartScreen() {
  screen.value = 'start'
}

function showHighScores() {
  loadHighScores() // Carga los récords antes de mostrarlos
  screen.value = 'records'
}

function showRecordsAndSave() {
  saveHighScore() // Guarda el récord actual
  showHighScores() // Luego muestra la tabla de récords
}

// --- Funciones del Juego ---

// Carga las preguntas desde la API
async function fetchQuestions() {
  isLoadingQuestions.value = true
  questionsLoadError.value = null // Resetea cualquier error previo
  try {
    const response = await axios.get(QUESTIONS_API_URL)
    allFetchedQuestions.value = response.data

    // Validaciones básicas de las preguntas
    if (allFetchedQuestions.value.length < prizeValues.value.length) {
      questionsLoadError.value =
        'Advertencia: No hay suficientes preguntas en la base de datos para jugar una partida completa (requerido: 15).'
      console.warn(questionsLoadError.value)
    }

    const invalidQuestions = allFetchedQuestions.value.filter((q) => q.options.length !== 4)
    if (invalidQuestions.length > 0) {
      questionsLoadError.value =
        'Algunas preguntas no tienen 4 opciones. Por favor, revisa los datos.'
      console.error('Preguntas con opciones incompletas:', invalidQuestions)
    }
  } catch (error) {
    console.error('Error al cargar preguntas de la API:', error)
    questionsLoadError.value =
      'No se pudieron cargar las preguntas del juego. Asegúrate de que el servidor API esté corriendo y la URL sea correcta.'
  } finally {
    isLoadingQuestions.value = false
  }
}

// Inicia un nuevo juego
function startGame() {
  // Prevención si hay errores o no hay preguntas
  if (isLoadingQuestions.value || questionsLoadError.value) {
    alert(
      'Las preguntas aún se están cargando o hubo un error. Por favor, espera o resuelve el problema.',
    )
    return
  }
  if (allFetchedQuestions.value.length === 0) {
    alert(
      'No hay preguntas disponibles para iniciar el juego. Por favor, recarga la página o contacta al administrador.',
    )
    return
  }

  shuffledQuestions.value = [] // Resetea las preguntas de la partida

  // Agrupa las preguntas por su nivel de dificultad
  const questionsByDifficulty = allFetchedQuestions.value.reduce((acc, q) => {
    const difficulty = q.nivelDificultad // Asume que la API devuelve 'nivelDificultad' (camelCase)
    // Si tu API devuelve 'NivelDificultad' (PascalCase), cambia la línea de arriba a:
    // const difficulty = q.NivelDificultad
    if (!acc[difficulty]) {
      acc[difficulty] = []
    }
    acc[difficulty].push(q)
    return acc
  }, {})

  // Selecciona las preguntas para la partida basándose en el difficultyMap
  for (let i = 0; i < prizeValues.value.length; i++) {
    const requiredDifficulty = difficultyMap[i]
    let availableQuestionsForThisDifficulty = questionsByDifficulty[requiredDifficulty]

    // Verifica si hay preguntas disponibles para la dificultad requerida
    if (!availableQuestionsForThisDifficulty || availableQuestionsForThisDifficulty.length === 0) {
      questionsLoadError.value = `Error: No hay preguntas de dificultad ${requiredDifficulty} en la base de datos para el nivel ${i + 1}.`
      alert(questionsLoadError.value + ' El juego no puede iniciar. Por favor, revisa los datos.')
      screen.value = 'start'
      return
    }

    // Filtra las preguntas para no repetir en la misma partida
    let candidates = availableQuestionsForThisDifficulty.filter(
      (q) => !shuffledQuestions.value.some((sq) => sq.id === q.id),
    )

    // Si no quedan preguntas únicas, reutiliza las existentes (esto es una lógica de fallback)
    if (candidates.length === 0) {
      console.warn(
        `Advertencia: No quedan preguntas únicas de dificultad ${requiredDifficulty} para el nivel ${i + 1}. Reutilizando preguntas existentes.`,
      )
      candidates = availableQuestionsForThisDifficulty // Vuelve a usar todas las de esa dificultad
      if (candidates.length === 0) {
        questionsLoadError.value = `Error crítico: No hay preguntas disponibles para la dificultad ${requiredDifficulty} (incluso reusando).`
        alert(questionsLoadError.value + ' El juego no puede iniciar.')
        screen.value = 'start'
        return
      }
    }

    // Selecciona una pregunta aleatoria de los candidatos
    const randomIndex = Math.floor(Math.random() * candidates.length)
    const selectedQuestion = candidates[randomIndex]

    shuffledQuestions.value.push(selectedQuestion) // Añade la pregunta seleccionada a la lista de la partida
  }

  // Inicializa el estado del juego
  screen.value = 'game'
  current.value = 0
  finalPrize.value = 0
  fiftyUsed.value = false
  audienceUsed.value = false
  callUsed.value = false
  visibleAnswers.value = [0, 1, 2, 3]
  nextQuestion() // Carga la primera pregunta
}

function restartGame() {
  // Reinicia el juego volviendo a la pantalla de inicio
  showStartScreen()
}

// Avanza a la siguiente pregunta o termina el juego
function nextQuestion() {
  if (current.value >= shuffledQuestions.value.length) {
    // Si se respondieron todas las preguntas, el jugador gana el premio máximo
    finalPrize.value = prizeValues.value[prizeValues.value.length - 1]
    endGame(true) // Gana
    return
  }

  time.value = 30 // Reinicia el temporizador
  visibleAnswers.value = [0, 1, 2, 3] // Resetea las respuestas visibles (si se usó 50/50)
  if (timer) clearInterval(timer) // Limpia cualquier temporizador anterior
  timer = setInterval(() => {
    time.value--
    if (time.value <= 0) {
      clearInterval(timer)
      endGame(false) // Pierde por tiempo
    }
  }, 1000)
}

// Comprueba la respuesta del jugador
function checkAnswer(index, isCorrect) {
  clearInterval(timer) // Detiene el temporizador
  if (isCorrect) {
    current.value++ // Avanza al siguiente nivel de pregunta
    nextQuestion() // Carga la siguiente pregunta
  } else {
    endGame(false) // Termina el juego si la respuesta es incorrecta
  }
}

// Termina el juego, calcula el premio final y cambia de pantalla
function endGame(win) {
  screen.value = 'end'
  if (win) {
    finalPrize.value = prizeValues.value[prizeValues.value.length - 1] // Gana el premio máximo
  } else {
    // Si pierde, gana el premio del "safe haven" si lo superó, de lo contrario 0
    if (current.value > safeHavenIndex) {
      finalPrize.value = prizeValues.value[safeHavenIndex]
    } else {
      finalPrize.value = 0
    }
  }
}

// --- Comodines ---
function useFiftyFifty() {
  if (fiftyUsed.value || !currentQuestion.value) return
  fiftyUsed.value = true

  const correctOptionIndex = currentQuestion.value.options.findIndex((opt) => opt.isCorrect)

  const incorrectOptionIndices = [0, 1, 2, 3].filter((i) => i !== correctOptionIndex)

  const optionsToHide = []
  while (optionsToHide.length < 2) {
    const randomIndex = Math.floor(Math.random() * incorrectOptionIndices.length)
    const optionIndex = incorrectOptionIndices[randomIndex]
    if (!optionsToHide.includes(optionIndex)) {
      optionsToHide.push(optionIndex)
    }
  }

  visibleAnswers.value = [
    correctOptionIndex,
    ...[0, 1, 2, 3].filter((i) => !optionsToHide.includes(i) && i !== correctOptionIndex),
  ].sort((a, b) => a - b) // Muestra la correcta y una incorrecta aleatoria
}

function askAudience() {
  if (audienceUsed.value || !currentQuestion.value) return
  audienceUsed.value = true

  const correctOptionIndex = currentQuestion.value.options.findIndex((opt) => opt.isCorrect)
  const votes = [0, 0, 0, 0]
  let remainingPercentage = 100

  // La respuesta correcta siempre tendrá el mayor porcentaje
  const correctVote = Math.floor(Math.random() * (70 - 40 + 1)) + 40 // Entre 40% y 70%
  votes[correctOptionIndex] = correctVote
  remainingPercentage -= correctVote

  const incorrectOptions = [0, 1, 2, 3].filter((i) => i !== correctOptionIndex)

  // Distribuye el resto de los votos entre las incorrectas
  for (let i = 0; i < incorrectOptions.length; i++) {
    const optionIndex = incorrectOptions[i]
    let vote
    if (i === incorrectOptions.length - 1) {
      // Asigna el resto a la última opción incorrecta para asegurar que suma 100
      vote = remainingPercentage
    } else {
      vote = Math.floor(Math.random() * (remainingPercentage / (incorrectOptions.length - i) + 1))
    }
    votes[optionIndex] = vote
    remainingPercentage -= vote
  }

  // Ajuste final para asegurar que la suma sea 100 (por si acaso hay desviaciones por redondeo)
  const totalVotes = votes.reduce((sum, v) => sum + v, 0)
  if (totalVotes !== 100) {
    votes[correctOptionIndex] += 100 - totalVotes // Ajusta la respuesta correcta si es necesario
  }

  alert(`Votación del Público:\nA: ${votes[0]}%\nB: ${votes[1]}%\nC: ${votes[2]}%\nD: ${votes[3]}%`)
}

function callFriend() {
  if (callUsed.value || !currentQuestion.value) return
  callUsed.value = true

  const correctOptionIndex = currentQuestion.value.options.findIndex((opt) => opt.isCorrect)
  const friendAnswers = []

  friendAnswers.push(correctOptionIndex) // El amigo siempre considera la respuesta correcta

  // 25% de probabilidad de que el amigo también sugiera una respuesta incorrecta
  if (Math.random() < 0.25) {
    const incorrectOptions = [0, 1, 2, 3].filter((i) => i !== correctOptionIndex)
    const randomIncorrectIndex = Math.floor(Math.random() * incorrectOptions.length)
    friendAnswers.push(incorrectOptions[randomIncorrectIndex])
  }

  // El amigo elige una de sus respuestas (puede ser la correcta o una incorrecta si añadió una)
  const friendFinalAnswerIndex = friendAnswers[Math.floor(Math.random() * friendAnswers.length)]

  alert(`Tu amigo cree que la respuesta es: ${['A', 'B', 'C', 'D'][friendFinalAnswerIndex]}`)
}

// --- Clases Dinámicas para Estilos ---
function answerClass(index) {
  const baseClass = ['a', 'b', 'c', 'd'][index]
  return {
    [baseClass]: true,
    // Puedes añadir lógica aquí para 'correct' o 'incorrect' en el futuro
  }
}

function prizeClass(index) {
  const reversedPrizeValues = [...prizeValues.value].reverse()
  const currentPrizeIndexOriginal = current.value
  const currentPrizeIndexInReversed = reversedPrizeValues.length - 1 - currentPrizeIndexOriginal

  // Estilos para la pantalla de fin de juego (resalta el premio final ganado)
  if (screen.value === 'end') {
    if (finalPrize.value === prizeValues.value[prizeValues.value.length - 1]) {
      // Si ganó el millón
      return { won: true }
    } else {
      // Si ganó un premio intermedio (o 0), resalta el premio seguro o el premio final
      const finalPrizeIndexOriginal = prizeValues.value.indexOf(finalPrize.value)
      // Calcula el índice del premio final en la escalera invertida
      const finalPrizeIndexInReversed = reversedPrizeValues.length - 1 - finalPrizeIndexOriginal
      return { won: index >= finalPrizeIndexInReversed && finalPrize.value > 0 }
    }
  }

  // Estilos durante el juego (resalta el premio actual y los premios ya ganados)
  return {
    current: index === currentPrizeIndexInReversed, // Premio actual
    won: index > currentPrizeIndexInReversed, // Premios ya superados
  }
}

// --- Manejo de Récords (localStorage) ---
function loadHighScores() {
  const storedScores = localStorage.getItem('millonarioHighScores')
  if (storedScores) {
    try {
      highScores.value = JSON.parse(storedScores)
    } catch (e) {
      console.error('Error parsing high scores from localStorage:', e)
      // Si hay un error de parseo, limpia el localStorage para evitar problemas futuros
      localStorage.removeItem('millonarioHighScores')
      highScores.value = []
    }
  } else {
    highScores.value = []
  }
}

function saveHighScore() {
  if (finalPrize.value > 0) {
    // Solo guarda si el premio final es mayor que 0
    const newScore = {
      player: currentUser.value, // Nombre del jugador (obtenido del authStore)
      prize: finalPrize.value,
      date: new Date().toISOString(), // Fecha y hora del récord
    }
    highScores.value.push(newScore)
    // Ordena los récords de mayor a menor y limita a los 10 mejores
    highScores.value.sort((a, b) => b.prize - a.prize)
    if (highScores.value.length > 10) {
      highScores.value = highScores.value.slice(0, 10)
    }
    localStorage.setItem('millonarioHighScores', JSON.stringify(highScores.value))
  }
}

// --- Ciclo de Vida ---
onMounted(() => {
  fetchQuestions() // Carga las preguntas al montar el componente
  loadHighScores() // Carga los récords al montar el componente
})
</script>

<style scoped>
/* --- Estilos Generales y Comunes --- */
.game-container {
  position: relative;
  width: 100vw;
  height: 100vh;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  background: radial-gradient(circle at center, #0b1c3d, #000);
  /* Degradado de fondo */
}

.start-screen,
.end-screen,
.records-screen {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  background: radial-gradient(circle at center, #0b1c3d, #000);
  z-index: 100;
  color: white;
}

.start-button,
.restart-button {
  background: linear-gradient(145deg, #222, #333);
  color: gold;
  border: 2px solid gold;
  padding: 15px 30px;
  border-radius: 50px;
  font-size: 1.5em;
  font-weight: bold;
  cursor: pointer;
  box-shadow: 0 0 10px gold;
  transition: all 0.3s ease;
  margin: 15px;
}
.start-button:hover,
.restart-button:hover {
  background: gold;
  color: black;
  box-shadow: 0 0 15px yellow;
  transform: scale(1.05);
}
.start-button:disabled {
  background: #444;
  border-color: #888;
  color: #aaa;
  box-shadow: none;
  cursor: not-allowed;
}

.title {
  font-size: clamp(2.5em, 5vw, 3.5em);
  color: gold;
  margin-bottom: 20px;
  text-shadow: 0 0 15px rgba(255, 215, 0, 0.8);
  text-align: center;
  line-height: 1.2;
}

.loading-message {
  color: #ffd700;
  font-size: 1.2em;
  margin-bottom: 20px;
}

.error-message {
  color: #f44336;
  font-size: 1.2em;
  margin-bottom: 20px;
  font-weight: bold;
  text-align: center;
}

/* --- Estilos para la Pantalla de Juego --- */
.top-info {
  position: absolute;
  top: 20px;
  width: 95%;
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  padding: 0 20px;
  box-sizing: border-box;
}

.logo {
  position: absolute;
  top: 10%;
  left: 50%;
  transform: translateX(-50%);
  width: clamp(200px, 25vw, 350px);
  max-width: 350px;
  z-index: 1;
}

.timer,
.current-prize {
  background: #222;
  border: 2px solid gold;
  border-radius: 10px;
  padding: 10px 20px;
  font-size: 2em;
  font-weight: bold;
  color: #ffd700;
  white-space: nowrap;
}

.question-text {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 80%;
  max-width: 800px;
  background: #132c64;
  padding: 20px;
  border-radius: 20px;
  text-align: center;
  font-size: clamp(1.2em, 2.5vw, 2em);
  font-weight: bold;
  color: #ffd700;
  box-shadow: 0 0 15px #000;
  z-index: 2;
}

.lifeline-container {
  position: absolute;
  top: 65%;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  gap: 20px;
  justify-content: center;
  z-index: 3;
}

.lifeline {
  background: linear-gradient(145deg, #222, #333);
  color: gold;
  border: 2px solid gold;
  padding: 12px 20px;
  border-radius: 50px;
  font-size: 1.1em;
  font-weight: bold;
  cursor: pointer;
  box-shadow: 0 0 10px gold;
  transition: all 0.3s ease;
}
.lifeline:hover {
  background: gold;
  color: black;
  box-shadow: 0 0 15px yellow;
}
.lifeline:disabled {
  background: #444;
  border-color: #888;
  color: #aaa;
  box-shadow: none;
  cursor: not-allowed;
}

.answers-grid {
  position: absolute;
  bottom: 5%;
  left: 50%;
  transform: translateX(-50%);
  width: 90%;
  max-width: 1000px;
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 20px;
  z-index: 4;
}

.answer {
  font-size: clamp(1em, 1.8vw, 1.2em);
  font-weight: bold;
  padding: 15px 20px;
  background: #1a1a1a;
  border: 3px solid #ffd700;
  border-radius: 12px;
  color: white;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 0 10px #000;
  text-align: left;
  display: flex;
  align-items: center;
  gap: 10px;
}
.answer:hover {
  background-color: #ffd700;
  color: #000;
  transform: scale(1.03);
}

.prize-ladder {
  position: absolute;
  left: 20px;
  top: 50%;
  transform: translateY(-50%);
  background: rgba(0, 0, 0, 0.7);
  padding: 10px;
  border-radius: 10px;
  border: 2px solid gold;
  max-height: 90vh;
  overflow-y: auto;
  min-width: 150px;
  z-index: 5;
}

.prize-ladder ul {
  list-style-type: none;
  padding: 0;
  margin: 0;
}

.prize-ladder li {
  padding: 5px 10px;
  margin: 2px 0;
  border-radius: 5px;
  text-align: right;
  font-size: 1.1em;
}

.prize-ladder li.current {
  background: gold;
  color: black;
  font-weight: bold;
}

.prize-ladder li.won {
  background: #132c64;
  color: gold;
  text-shadow: 0 0 5px rgba(255, 215, 0, 0.5);
}

/* Animaciones para respuestas */
.correct {
  animation: correctAnswer 1s;
}

.incorrect {
  animation: incorrectAnswer 1s;
}

@keyframes correctAnswer {
  0% {
    background: #1a1a1a;
  }
  50% {
    background: #4caf50;
  }
  100% {
    background: #1a1a1a;
  }
}

@keyframes incorrectAnswer {
  0% {
    background: #1a1a1a;
  }
  50% {
    background: #f44336;
  }
  100% {
    background: #1a1a1a;
  }
}

/* Estilos de Fin de Juego */
.final-prize {
  font-size: clamp(1.8em, 3vw, 2.5em);
  color: gold;
  margin: 20px 0;
  text-align: center;
  text-shadow: 0 0 10px rgba(255, 215, 0, 0.6);
}

/* Estilos de Tabla de Récords */
.no-records {
  font-size: 1.5em;
  color: #ccc;
  margin-top: 20px;
}

.high-scores-table {
  width: 80%;
  max-width: 600px;
  margin-top: 30px;
  border-collapse: collapse;
  background: rgba(0, 0, 0, 0.7);
  border: 2px solid gold;
  border-radius: 10px;
  overflow: hidden;
  box-shadow: 0 0 20px rgba(255, 215, 0, 0.5);
}

.high-scores-table th,
.high-scores-table td {
  padding: 12px 15px;
  text-align: left;
  border-bottom: 1px solid #444;
  color: white;
}

.high-scores-table th {
  background: #132c64;
  color: gold;
  font-size: 1.2em;
  text-transform: uppercase;
}

.high-scores-table tbody tr:nth-child(even) {
  background: rgba(0, 0, 0, 0.5);
}

.high-scores-table tbody tr:hover {
  background: rgba(255, 215, 0, 0.2);
}

/* Animación de victoria (no usada en el código actual, pero útil si se quiere añadir) */
@keyframes explode {
  0% {
    transform: scale(1);
    opacity: 1;
  }
  100% {
    transform: scale(10);
    opacity: 0;
  }
}
.win-effect {
  position: fixed;
  top: 50%;
  left: 50%;
  width: 100px;
  height: 100px;
  background: gold;
  border-radius: 50%;
  animation: explode 1s forwards;
  z-index: 1000;
}
</style>
