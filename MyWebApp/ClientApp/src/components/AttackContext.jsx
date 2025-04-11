import React, { createContext, useState } from "react";

export const AttackContext = createContext();

export const AttackProvider = ({ children }) => {
  const [attackPairs, setAttackPairs] = useState([]);
  const [attackName, setAttackName] = useState("Swing");
  const [value, setValue] = useState("");
  const [imageUrl, setImageUrl] = useState("");

  return (
    <AttackContext.Provider
      value={{
        attackPairs,
        setAttackPairs,
        attackName,
        setAttackName,
        value,
        setValue,
        imageUrl,
        setImageUrl,
      }}
    >
      {children}
    </AttackContext.Provider>
  );
};
