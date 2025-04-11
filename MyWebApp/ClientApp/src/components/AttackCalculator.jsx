import React, { useContext, useEffect, useRef, useState } from "react";
import { DndProvider, useDrag, useDrop } from "react-dnd";
import { HTML5Backend } from "react-dnd-html5-backend";
import axios from "axios";
import { AttackContext } from "./AttackContext.jsx";
import { DeckContext } from "./DeckContext.jsx";
import "./AttackCalculator.css";

const baseURL = import.meta.env.VITE_API_BASE || "";
const ItemType = "ATTACK";

const colorPalette = [
  "#4b5563", // muted slate
  "#5b4b8a", // dark violet
  "#3a5f5c", // desaturated teal
  "#3a4f6c", // navy steel
  "#5c473a", // warm taupe
  "#6b3b3b", // muted wine
  "#5a4d6d", // dusty purple
  "#4d3e50", // charcoal lavender
  "#3f5d5b", // deep pine
  "#5c6145"  // mossy olive
];

const getColorFromName = (name) => {
  let hash = 0;
  for (let i = 0; i < name.length; i++) {
    hash = name.charCodeAt(i) + ((hash << 5) - hash);
  }
  return colorPalette[Math.abs(hash) % colorPalette.length];
};

const AttackItem = ({ pair, index, moveAttack, removeAttack, updateAttack }) => {
  const [isEditing, setIsEditing] = useState(false);
  const [editedArgs, setEditedArgs] = useState(pair.Args || []);
  const bgColor = getColorFromName(pair.AttackName);

  const [{ isDragging }, ref] = useDrag({
    type: ItemType,
    item: { index },
    collect: (monitor) => ({ isDragging: !!monitor.isDragging() }),
  });

  const [, drop] = useDrop({
    accept: ItemType,
    hover: (draggedItem) => {
      if (draggedItem.index !== index) {
        moveAttack(draggedItem.index, index);
        draggedItem.index = index;
      }
    },
  });

  const handleEdit = () => setIsEditing(true);
  const handleCancel = () => {
    setIsEditing(false);
    setEditedArgs(pair.Args);
  };

  const handleSave = () => {
    updateAttack(index, editedArgs);
    setIsEditing(false);
  };

  const handleChange = (i, value) => {
    const newArgs = [...editedArgs];
    newArgs[i] = value;
    setEditedArgs(newArgs);
  };

  return (
    <li
      ref={(node) => ref(drop(node))}
      className={`attack-item ${isDragging ? "dragging" : ""}`}
      style={{ backgroundColor: bgColor }}
    >
      <div className="attack-box">
        <span className="attack-text">
          <strong>{pair.AttackName}:</strong>{" "}
          {isEditing ? (
            editedArgs.map((val, i) => (
              <input
                key={i}
                type="number"
                value={val}
                onChange={(e) => handleChange(i, e.target.value)}
                className="attack-input"
                style={{ width: "60px", marginRight: "5px" }}
              />
            ))
          ) : (
            pair.Args?.join(", ")
          )}
        </span>
        <div style={{ display: "flex", gap: "5px", alignItems: "center" }}>
          <button
            onClick={() => moveAttack(index, index - 1)}
            disabled={index === 0}
            className="up-button"
          >
            ⬆
          </button>

          <button
            onClick={() => moveAttack(index, index + 1)}
            disabled={index === pair.total - 1}
            className="down-button"
          >
            ⬇
          </button>

          {isEditing ? (
            <>
              <button onClick={handleSave}>✔</button>
              <button onClick={handleCancel}>✖</button>
            </>
          ) : (
            <>
              <button onClick={handleEdit}>✎</button>
              <button onClick={() => removeAttack(index)} className="remove-button">✖</button>
            </>
          )}
        </div>
      </div>
    </li>
  );
};

const AttackCalculator = () => {
  const {
    attackPairs, setAttackPairs,
    attackName, setAttackName,
    imageUrl, setImageUrl
  } = useContext(AttackContext);

  const { decks } = useContext(DeckContext);

  const [basicMethods, setBasicMethods] = useState([]);
  const [finisherMethods, setFinisherMethods] = useState([]);
  const [showFinishers, setShowFinishers] = useState(false);
  const [argValues, setArgValues] = useState([]);
  const [isCooldown, setIsCooldown] = useState(false);

  const listRef = useRef(null);

  useEffect(() => {
    const fetchMethods = async () => {
      try {
        const [basicRes, finisherRes] = await Promise.all([
          axios.get(`${baseURL}/api/attack/basic-methods`),
          axios.get(`${baseURL}/api/attack/finisher-methods`)
        ]);

        const sortedBasic = basicRes.data.sort((a, b) => a.method.localeCompare(b.method));
        const sortedFinisher = finisherRes.data.sort((a, b) => a.method.localeCompare(b.method));

        setBasicMethods(sortedBasic);
        setFinisherMethods(sortedFinisher);

        setAttackName(sortedBasic[0].method);
        setArgValues(Array(sortedBasic[0].parameters.length).fill(""));
      } catch (error) {
        console.error("Failed to fetch methods", error);
      }
    };

    fetchMethods();
  }, []);

  useEffect(() => {
    if (listRef.current) {
      listRef.current.scrollTop = listRef.current.scrollHeight;
    }
  }, [attackPairs]);

  const handleInputChange = (event) => {
    const { name, value } = event.target;
    if (name === "attackName") {
      setAttackName(value);
      const source = showFinishers ? finisherMethods : basicMethods;
      const paramCount = source.find(m => m.method === value)?.parameters.length || 0;
      setArgValues(Array(paramCount).fill(""));
    }
  };

  const handleArgChange = (index, value) => {
    const updated = [...argValues];
    updated[index] = value;
    setArgValues(updated);
  };

  const addAttackPair = () => {
    if (!attackName || argValues.some(val => val === "")) return;
    const parsedArgs = argValues.map(v => parseInt(v, 10));
    setAttackPairs([...attackPairs, { AttackName: attackName, Args: parsedArgs }]);
  };

  const removeAttack = (index) => {
    setAttackPairs(attackPairs.filter((_, i) => i !== index));
  };

  const moveAttack = (fromIndex, toIndex) => {
    if (toIndex < 0 || toIndex >= attackPairs.length) return;
    const updated = [...attackPairs];
    const [moved] = updated.splice(fromIndex, 1);
    updated.splice(toIndex, 0, moved);
    setAttackPairs(updated);
  };

  const submitAttackRequest = async () => {
    if (attackPairs.length === 0) return;

    const calculateTotalCards = (deck) =>
      Number(deck.Lv0InDeck) +
      Number(deck.Lv1InDeck) +
      Number(deck.Lv2InDeck) +
      Number(deck.Lv3InDeck) +
      Number(deck.CXInDeck);

    const fullRequest = {
      attackNameValuePairs: attackPairs,
      selfDeckInfo: {
        ...decks.SelfDeckInfo,
        totalCardsInDeck: calculateTotalCards(decks.SelfDeckInfo),
      },
      oppDeckInfo: {
        ...decks.OppDeckInfo,
        totalCardsInDeck: calculateTotalCards(decks.OppDeckInfo),
      },
      opp2ndDeckInfo: {
        ...decks.Opp2ndDeckInfo,
        totalCardsInDeck: calculateTotalCards(decks.Opp2ndDeckInfo),
      },
    };

    try {
      const response = await axios.post(
        `${baseURL}/api/attack/calculate-damage`,
        fullRequest,
        { responseType: "blob" }
      );
      setImageUrl(URL.createObjectURL(response.data));
    } catch (error) {
      if (error.response?.status === 500 && error.response.data instanceof Blob) {
        const reader = new FileReader();
        reader.onload = () => {
          alert("Server error:\n" + reader.result);
        };
        reader.readAsText(error.response.data);
      } else {
        alert("Unexpected error: " + error.message);
      }
    }
  };

  const methodsToShow = showFinishers ? finisherMethods : basicMethods;
  const currentParams = methodsToShow.find(m => m.method === attackName)?.parameters || [];

  return (
    <DndProvider backend={HTML5Backend}>
      <div className="attack-calculator-container">
        <div className="attack-main-content">
          <div className="attack-calculator">
            <h2>Attack Damage Calculator</h2>
            <div className="toggle-container">
              <label className="toggle-label">
                <input
                  type="checkbox"
                  checked={showFinishers}
                  onChange={() => {
                    const newMode = !showFinishers;
                    setShowFinishers(newMode);

                    const newMethods = newMode ? finisherMethods : basicMethods;
                    if (newMethods.length > 0) {
                      setAttackName(newMethods[0].method);
                      setArgValues(Array(newMethods[0].parameters.length).fill(""));
                    }
                  }}
                />
                <span className="toggle-slider" />
                <span className="toggle-text">
                  {showFinishers ? "Showing Finishers" : "Showing Basics"}
                </span>
              </label>
            </div>

            <select
              name="attackName"
              value={attackName}
              onChange={handleInputChange}
              className="attack-dropdown"
            >
              {methodsToShow.map((method) => (
                <option key={method.method} value={method.method}>
                  {method.method} {method.parameters.length > 0 ? `(${method.parameters.join(", ")})` : ""}
                </option>
              ))}
            </select>

            <div className="text-sm text-gray-400 mb-2">
              {currentParams.length > 0
                ? `Parameters: ${currentParams.join(", ")}`
                : "Choose a method to see required inputs"}
            </div>

            {currentParams.map((param, index) => (
              <input
                key={index}
                type="number"
                placeholder={param}
                value={argValues[index] || ""}
                onChange={(e) => handleArgChange(index, e.target.value)}
                className="attack-input"
              />
            ))}

            <button onClick={addAttackPair}>Add Attack</button>

            <div className="attack-list-container" ref={listRef}>
              <ul className="attack-list">
                {attackPairs.map((pair, index) => (
                  <AttackItem
                    key={`${pair.AttackName}-${index}`}
                    index={index}
                    pair={{ ...pair, total: attackPairs.length }}
                    moveAttack={moveAttack}
                    removeAttack={removeAttack}
                    updateAttack={(i, newArgs) => {
                      const updated = [...attackPairs];
                      updated[i].Args = newArgs.map((v) => parseInt(v, 10));
                      setAttackPairs(updated);
                    }}
                  />
                ))}
              </ul>
            </div>

            <button
              onClick={() => {
                if (isCooldown) return;
                setIsCooldown(true);
                setTimeout(() => setIsCooldown(false), 300);
                submitAttackRequest();
              }}
              disabled={isCooldown}
              style={{ opacity: isCooldown ? 0.5 : 1 }}
            >
              Calculate Damage
            </button>
          </div>

          {imageUrl && (
            <div className="attack-graph-container">
              <h3>Generated Graph:</h3>
              <img src={imageUrl} alt="Damage Graph" className="attack-graph-image" />
            </div>
          )}
        </div>
      </div>
    </DndProvider>
  );
};

export default AttackCalculator;