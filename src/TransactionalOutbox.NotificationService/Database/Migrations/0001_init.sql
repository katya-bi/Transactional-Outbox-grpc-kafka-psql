CREATE TABLE notifications (
    id              TEXT PRIMARY KEY,
    user_id         TEXT NOT NULL,
    order_id        TEXT NOT NULL,
    type            INTEGER NOT NULL,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE INDEX ix_notifications_user_id
    ON notifications (user_id);