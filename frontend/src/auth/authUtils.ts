export type UserRole = "Admin" | "Manager" | "Employee";

export interface AuthUser {
  userId: string;
  role: UserRole;
  firstName: string;
  lastName: string;
}

const ROLE_CLAIM =
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

export const getUserFromToken = (
  token: string | null,
): AuthUser | null => {
  if (!token) {
    return null;
  }

  try {
    const payload = token.split(".")[1];

    if (!payload) {
      return null;
    }

    const decoded = JSON.parse(
      atob(payload.replace(/-/g, "+").replace(/_/g, "/")),
    );

    const userId = decoded.employeeId;
    const role = decoded[ROLE_CLAIM];

    if (!userId || !role) {
      console.error("Required authentication claims are missing.");
      return null;
    }

    return {
      userId,
      role: role as UserRole,
      firstName: decoded.firstName ?? "",
      lastName: decoded.lastName ?? "",
    };
  } catch (error) {
    console.error("Failed to decode JWT:", error);
    return null;
  }
};