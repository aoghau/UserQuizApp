import "./App.css";
import "./styles/styles.scss";
import { Routes, Route } from "react-router-dom";
import Login from "./components/Login";
import Home from "./components/Home";
import Quiz from "./components/Quiz";
import Results from "./components/Results";

function App() {
  return (
    <Routes>
      <Route path="/" element={<Login />} />
      <Route path="/home" element={<Home />} />
      <Route path="/quiz/:id" element={<Quiz />} />
      <Route path="/results/:id" element={<Results />} />
    </Routes>
  );
}

export default App;
