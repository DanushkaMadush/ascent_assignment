const ROLE_CLAIM =
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

export type Role = "Admin" | "Manager" | "Employee";

export const getRoleFromToken = (token: string): Role | null => {
  try {
    const payload = token.split(".")[1];

    const decodedPayload = JSON.parse(
      atob(
        payload
          .replace(/-/g, "+")
          .replace(/_/g, "/")
      )
    );

    return decodedPayload[ROLE_CLAIM] ?? null;
  } catch {
    return null;
  }
};