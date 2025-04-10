import React, { useContext, useEffect, useState } from "react";
import { DndProvider, useDrag, useDrop } from "react-dnd";
import { HTML5Backend } from "react-dnd-html5-backend";
import axios from "axios";
import { AttackContext } from "./AttackContext.jsx";
import "./AttackCalculator.css";

const ItemType = "ATTACK";

const AttackItem = ({ pair, index, moveAttack, removeAttack, updateAttack }) => {
  const [isEditing, setIsEditing] = useState(false);
  const [editedArgs, setEditedArgs] = useState(pair.Args || []);

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
    <li ref={(node) => ref(drop(node))} className={`attack-item ${isDragging ? "dragging" : ""}`}>
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
        <div style={{ display: "flex", gap: "5px" }}>
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

  const [basicMethods, setBasicMethods] = useState([]);
  const [finisherMethods, setFinisherMethods] = useState([]);
  const [showFinishers, setShowFinishers] = useState(false);
  const [argValues, setArgValues] = useState([]);

  useEffect(() => {
    const fetchMethods = async () => {
      try {
        const [basicRes, finisherRes] = await Promise.all([
          axios.get("https://localhost:7060/api/attack/basic-methods"),
          axios.get("https://localhost:7060/api/attack/finisher-methods")
        ]);

        const sortedBasic = basicRes.data.sort((a, b) => a.method.localeCompare(b.method));
        const sortedFinisher = finisherRes.data.sort((a, b) => a.method.localeCompare(b.method));

        setBasicMethods(sortedBasic);
        setFinisherMethods(sortedFinisher);

        // Set initial selection from basic methods
        setAttackName(sortedBasic[0].method);
        setArgValues(Array(sortedBasic[0].parameters.length).fill(""));
      } catch (error) {
        console.error("Failed to fetch methods", error);
      }
    };

    fetchMethods();
  }, []);



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
    //setArgValues(argValues.map(() => ""));
  };

  const removeAttack = (index) => {
    setAttackPairs(attackPairs.filter((_, i) => i !== index));
  };

  const moveAttack = (fromIndex, toIndex) => {
    const updated = [...attackPairs];
    const [moved] = updated.splice(fromIndex, 1);
    updated.splice(toIndex, 0, moved);
    setAttackPairs(updated);
  };

  const submitAttackRequest = async () => {
    if (attackPairs.length === 0) return;
    try {
      const response = await axios.post(
        "https://localhost:7060/api/attack/calculate-damage",
        { AttackNameValuePairs: attackPairs },
        { responseType: "blob" }
      );
      setImageUrl(URL.createObjectURL(response.data));
    } catch (error) {
      console.error("Failed to calculate damage.", error);
      if (error.response?.status === 500) {
        const reader = new FileReader();
        reader.onload = () => {
          alert(reader.result || "Decks not initialized. Please set them up first.");
        };
        reader.readAsText(error.response.data);
      } else {
        alert("An unexpected error occurred. Please try again.");
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
            <ul className="attack-list">
              {attackPairs.map((pair, index) => (
                <AttackItem
                  key={`${pair.AttackName}-${index}`}
                  index={index}
                  pair={pair}
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
            <button onClick={submitAttackRequest}>Calculate Damage</button>
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