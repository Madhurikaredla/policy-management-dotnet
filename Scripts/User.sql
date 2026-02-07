CREATE TABLE IF NOT EXISTS users (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    role VARCHAR(20) NOT NULL,
    country_code VARCHAR(5),
    phone_number VARCHAR(30),
    created_at TIMESTAMP NOT NULL,
    updated_at TIMESTAMP NOT NULL,
    active BOOLEAN NOT NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS ix_users_email
ON users (email);

CREATE INDEX IF NOT EXISTS ix_users_phone_number_country_code
ON users (phone_number, country_code);

INSERT INTO users (name, email, password_hash, role, country_code, phone_number, created_at, updated_at, active)
VALUES
('John Doe', 'john@example.com', 'XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=', 'ROLE_USER', '+1', '0987654321', NOW(), NOW(), true),
('Admin User', 'admin@example.com', 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=', 'ROLE_ADMIN', '+1', '1234567890', NOW(), NOW(), true);


SELECT * FROM users;
