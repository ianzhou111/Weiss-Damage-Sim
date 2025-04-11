import React, { createContext, useState, useEffect } from "react";

export const DeckContext = createContext();

const defaultSelfDeck = {
  Lv0InDeck: "8",
  Lv1InDeck: "7",
  Lv2InDeck: "3",
  Lv3InDeck: "5",
  CXInDeck: "7",
  SoulTriggersInDeck: "8",
};

const defaultOppDeck = {
  Lv0InDeck: "7",
  Lv1InDeck: "3",
  Lv2InDeck: "4",
  Lv3InDeck: "4",
  CXInDeck: "7",
  SoulTriggersInDeck: "6",
};

const defaultOpp2ndDeck = {
  Lv0InDeck: "10",
  Lv1InDeck: "8",
  Lv2InDeck: "6",
  Lv3InDeck: "0",
  CXInDeck: "8",
  SoulTriggersInDeck: "8",
};

const getInitialDecks = () => {
  const saved = localStorage.getItem("weiss-decks");
  if (saved) {
    try {
      return JSON.parse(saved);
    } catch (e) {
      console.error("Failed to parse saved deck:", e);
    }
  }

  return {
    SelfDeckInfo: { ...defaultSelfDeck },
    OppDeckInfo: { ...defaultOppDeck },
    Opp2ndDeckInfo: { ...defaultOpp2ndDeck },
  };
};

export const DeckProvider = ({ children }) => {
  const [decks, setDecks] = useState(getInitialDecks);

  const resetToDefaults = () => {
    const defaultDecks = {
      SelfDeckInfo: { ...defaultSelfDeck },
      OppDeckInfo: { ...defaultOppDeck },
      Opp2ndDeckInfo: { ...defaultOpp2ndDeck },
    };
    setDecks(defaultDecks);
    localStorage.setItem("weiss-decks", JSON.stringify(defaultDecks));
  };

  useEffect(() => {
    localStorage.setItem("weiss-decks", JSON.stringify(decks));
  }, [decks]);

  return (
    <DeckContext.Provider value={{ decks, setDecks, resetToDefaults }}>
      {children}
    </DeckContext.Provider>
  );
};
