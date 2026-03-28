import React from "react";
import styles from "./Input.module.css";

const PassIn = ({ value, onChange, onEnter }) => {
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
                <span style={{ transitionDelay: "0ms" }}>P</span>
                <span style={{ transitionDelay: "50ms" }}>a</span>
                <span style={{ transitionDelay: "100ms" }}>s</span>
                <span style={{ transitionDelay: "150ms" }}>s</span>
                <span style={{ transitionDelay: "200ms" }}>w</span>
                <span style={{ transitionDelay: "250ms" }}>o</span>
                <span style={{ transitionDelay: "300ms" }}>r</span>
                <span style={{ transitionDelay: "350ms" }}>d</span>
            </label>
        </div>
    );
};

export default PassIn;