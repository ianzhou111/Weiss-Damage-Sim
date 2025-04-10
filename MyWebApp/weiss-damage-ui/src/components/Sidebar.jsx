import React from "react";
import { Link } from "react-router-dom";
import "./Sidebar.css";

const Sidebar = () => {
  return (
    <div className="sidebar open"> {/* Sidebar is always open */}
      <nav className="menu">
        <Link to="/">Attack Calculator</Link>
        <Link to="/setup-decks">Set Up Decks</Link>
      </nav>
    </div>
  );
};

export default Sidebar;
