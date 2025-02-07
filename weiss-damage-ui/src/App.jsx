import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import AttackCalculator from "./components/AttackCalculator";
import SetupDecks from "./components/SetupDecks";
import Sidebar from "./components/Sidebar";
import { AttackProvider } from "./components/AttackContext.jsx"; // Import Context Provider
import "./App.css";
import { DeckProvider } from "./components/DeckContext.jsx";

function App() {
  return (
    <DeckProvider>
      <AttackProvider>
        <Router>
          <div className="app-container">
            <Sidebar alwaysOpen={true} />
            <div className="content">
              <Routes>
                <Route path="/" element={<AttackCalculator />} />
                <Route path="/setup-decks" element={<SetupDecks />} />
              </Routes>
            </div>
          </div>
        </Router>
      </AttackProvider>
    </DeckProvider>
  );
}

export default App;
