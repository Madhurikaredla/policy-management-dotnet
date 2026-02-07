-- SQL script to create the policies table in the database
CREATE TABLE policies (
    id SERIAL PRIMARY KEY,
    policy_code VARCHAR(50) NOT NULL UNIQUE,
    policy_name VARCHAR(255) NOT NULL,
    description TEXT,
    premium_amount DECIMAL(10, 2) NOT NULL,
    is_active BOOLEAN,
    created_at TIMESTAMP,
    updated_at TIMESTAMP,
    deleted_at TIMESTAMP
);


INSERT INTO policies (policy_code, policy_name, description, premium_amount, is_active, created_at)
VALUES 
('POL001', 'Health Insurance', 'Comprehensive health insurance policy', 5000.00, TRUE, NOW()),
('POL002', 'Life Insurance', 'Term life insurance policy', 3000.00, TRUE, NOW()),
('POL003', 'Auto Insurance', 'Full coverage auto insurance policy', 4000.00, TRUE, NOW());