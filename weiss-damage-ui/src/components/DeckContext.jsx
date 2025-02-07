import React, { createContext, useState } from "react";

export const DeckContext = createContext();

export const DeckProvider = ({ children }) => {
  const [decks, setDecks] = useState({
    SelfDeckInfo: {
      Lv0InDeck: "",
      Lv1InDeck: "",
      Lv2InDeck: "",
      Lv3InDeck: "",
      CXInDeck: "",
      SoulTriggersInDeck: "",
    },
    OppDeckInfo: {
      Lv0InDeck: "",
      Lv1InDeck: "",
      Lv2InDeck: "",
      Lv3InDeck: "",
      CXInDeck: "",
      SoulTriggersInDeck: "",
    },
    Opp2ndDeckInfo: {
      Lv0InDeck: "",
      Lv1InDeck: "",
      Lv2InDeck: "",
      Lv3InDeck: "",
      CXInDeck: "",
      SoulTriggersInDeck: "",
    },
  });

  return (
    <DeckContext.Provider value={{ decks, setDecks }}>
      {children}
    </DeckContext.Provider>
  );
};
