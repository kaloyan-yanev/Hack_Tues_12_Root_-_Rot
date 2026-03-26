import React from "react";
import styles from "./Input.module.css";

const EmailIn = ({ value, onChange, onEnter }) => {
    const handleKeyDown = (e) => {
        if (e.key === "Enter") {
            onEnter(value);
        }
    };

    return (
        <div className={styles.formControl}>
            <input
                type="text"
                required
                value={value}
                onChange={onChange}
                onKeyDown={handleKeyDown}
                autoComplete="off"
            />
            <label>
                <span style={{ transitionDelay: "0ms" }}>E</span>
                <span style={{ transitionDelay: "50ms" }}>m</span>
                <span style={{ transitionDelay: "100ms" }}>a</span>
                <span style={{ transitionDelay: "150ms" }}>i</span>
                <span style={{ transitionDelay: "200ms" }}>l</span>
            </label>
        </div>
    );
};

export default EmailIn;