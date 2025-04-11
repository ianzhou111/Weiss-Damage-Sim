import { useContext } from "react";
import { DeckContext } from "./DeckContext";

// Helper to calculate total cards
const calculateTotalCards = (deck) => {
  return (
    Number(deck.Lv0InDeck || 0) +
    Number(deck.Lv1InDeck || 0) +
    Number(deck.Lv2InDeck || 0) +
    Number(deck.Lv3InDeck || 0) +
    Number(deck.CXInDeck || 0)
  );
};

const SetupDecks = () => {
  const { decks, setDecks, resetToDefaults } = useContext(DeckContext);

  const handleChange = (deck, e) => {
    setDecks((prevDecks) => ({
      ...prevDecks,
      [deck]: {
        ...prevDecks[deck],
        [e.target.name]: e.target.value,
      },
    }));
  };

  const handleValidate = () => {
    for (const deck in decks) {
      if (calculateTotalCards(decks[deck]) > 50) {
        alert(`${deck.replace(/([A-Z])/g, " $1").trim()} exceeds 50 cards!`);
        return;
      }
    }
    alert("✅ All deck totals are valid and saved locally.");
  };

  return (
    <div className="flex justify-center items-center h-screen bg-gray-900">
      <div className="bg-gray-800 text-white p-6 rounded-2xl shadow-lg w-full max-w-5xl">
        <h2 className="text-2xl font-bold text-center mb-4">Setup Decks</h2>
        <div className="grid grid-cols-3 gap-4">
          {Object.keys(decks).map((deck) => (
            <div key={deck} className="bg-gray-700 p-4 rounded-lg shadow-md flex flex-col items-center">
              <h3 className="text-xl font-semibold text-center mb-4 w-full">
                {deck.replace(/([A-Z])/g, " $1").trim()}
              </h3>
              <div className="grid grid-rows-6 gap-3 w-full">
                {Object.keys(decks[deck]).map((key) => (
                  <div key={key} className="flex flex-col items-center w-full">
                    <span className="font-semibold mb-1 text-center w-full">
                      {key.replace(/([A-Z])/g, " $1").trim()}
                    </span>
                    <input
                      type="number"
                      name={key}
                      value={decks[deck][key]}
                      onChange={(e) => handleChange(deck, e)}
                      placeholder={key.replace(/([A-Z])/g, " $1").trim()}
                      className="w-full p-2 bg-gray-600 border border-gray-500 rounded text-white placeholder-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-500 text-center"
                    />
                  </div>
                ))}
                <div className="font-bold mt-2">
                  Total Cards: {calculateTotalCards(decks[deck])}
                </div>
              </div>
            </div>
          ))}
        </div>

        <button
          className="mt-6 w-full bg-blue-500 hover:bg-blue-600 text-white font-semibold py-2 rounded"
          onClick={handleValidate}
        >
          Validate Deck Info
        </button>

        <button
          className="mt-3 w-full bg-red-500 hover:bg-red-600 text-white font-semibold py-2 rounded"
          onClick={resetToDefaults}
        >
          Reset to Default
        </button>
      </div>
    </div>
  );
};

export default SetupDecks;
