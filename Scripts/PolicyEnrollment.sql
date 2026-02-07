-- Create policy_enrollments table
CREATE TABLE IF NOT EXISTS policy_enrollments (
    id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL,
    policy_id INTEGER NOT NULL,
    status VARCHAR(20) NOT NULL DEFAULT 'Pending',
    requested_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    approved_at TIMESTAMP,
    rejected_at TIMESTAMP,
    admin_remarks VARCHAR(500),
    
    -- Foreign key constraints
    CONSTRAINT fk_enrollment_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE RESTRICT,
    CONSTRAINT fk_enrollment_policy FOREIGN KEY (policy_id) REFERENCES policies(id) ON DELETE RESTRICT,
    
    -- Unique constraint to prevent duplicate enrollments
    CONSTRAINT uq_user_policy UNIQUE (user_id, policy_id),
    
    -- Check constraint for status values
    CONSTRAINT chk_status CHECK (status IN ('Pending', 'Approved', 'Rejected'))
);

-- Create indexes for better query performance
CREATE INDEX idx_enrollment_user_id ON policy_enrollments(user_id);
CREATE INDEX idx_enrollment_policy_id ON policy_enrollments(policy_id);
CREATE INDEX idx_enrollment_status ON policy_enrollments(status);
CREATE INDEX idx_enrollment_requested_at ON policy_enrollments(requested_at);

-- Add unique index on email for users table if not exists
CREATE UNIQUE INDEX IF NOT EXISTS idx_users_email ON users(email);

-- Add unique index on policy_code for policies table if not exists
CREATE UNIQUE INDEX IF NOT EXISTS idx_policies_code ON policies(policy_code);
