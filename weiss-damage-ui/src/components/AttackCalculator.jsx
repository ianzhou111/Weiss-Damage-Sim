import React, { useContext } from "react";
import { DndProvider, useDrag, useDrop } from "react-dnd";
import { HTML5Backend } from "react-dnd-html5-backend";
import axios from "axios";
import { AttackContext } from "./AttackContext.jsx";
import "./AttackCalculator.css";

const ItemType = "ATTACK";

const AttackItem = ({ pair, index, moveAttack, removeAttack }) => {
  const [{ isDragging }, ref] = useDrag({
    type: ItemType,
    item: { index },
    collect: (monitor) => ({
      isDragging: !!monitor.isDragging(),
    }),
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

  return (
    <li
      ref={(node) => ref(drop(node))}
      className={`attack-item ${isDragging ? "dragging" : ""}`}
    >
      <div className="attack-box">
        <span className="attack-text">{pair.AttackName}: {pair.Value}</span>
        <button onClick={() => removeAttack(index)} className="remove-button">✖</button>
      </div>
    </li>
  );
};

const AttackCalculator = () => {
  const { attackPairs, setAttackPairs, attackName, setAttackName, value, setValue, imageUrl, setImageUrl } = useContext(AttackContext);
  
  const handleInputChange = (event) => {
    const { name, value } = event.target;
    if (name === "attackName") setAttackName(value);
    else setValue(value);
  };

  const addAttackPair = () => {
    const intValue = parseInt(value);
    if (attackName && intValue >= 1 && intValue <= 100) {
      setAttackPairs([...attackPairs, { AttackName: attackName, Value: intValue }]);
    }
  };

  const removeAttack = (index) => {
    setAttackPairs(attackPairs.filter((_, i) => i !== index));
  };

  const moveAttack = (fromIndex, toIndex) => {
    const updatedPairs = [...attackPairs];
    const [movedItem] = updatedPairs.splice(fromIndex, 1);
    updatedPairs.splice(toIndex, 0, movedItem);
    setAttackPairs(updatedPairs);
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
    }
  };

  return (
    <DndProvider backend={HTML5Backend}>
      <div className="attack-calculator-container">
        <div className="attack-main-content">
          <div className="attack-calculator">
            <h2>Attack Damage Calculator</h2>
            <select
              name="attackName"
              value={attackName}
              onChange={handleInputChange}
              className="attack-dropdown"
            >
              <option value="Swing">Swing</option>
              <option value="Burn">Burn</option>
            </select>
            <input
              type="number"
              name="value"
              placeholder={attackName === "Swing" ? "Soul Count" : "Amount"}
              value={value}
              min="1"
              max="100"
              onChange={handleInputChange}
              className="attack-input"
            />
            <button onClick={addAttackPair}>Add Attack</button>
            <ul className="attack-list">
              {attackPairs.map((pair, index) => (
                <AttackItem
                  key={index}
                  index={index}
                  pair={pair}
                  moveAttack={moveAttack}
                  removeAttack={removeAttack}
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
