import { useLocation, Navigate, Outlet } from "react-router-dom";
import { tokenStorage } from "../api/client/tokenStorage";
import { getRoleFromToken, type Role } from "../api/client/jwt";

interface AuthGuardProps {
  allowedRoles?: Role[];
}

export const AuthGuard = ({ allowedRoles }: AuthGuardProps) => {
  const location = useLocation();
  const accessToken = tokenStorage.getAccessToken();

  // User is not logged in
  if (!accessToken) {
    return (
      <Navigate
        to="/login"
        replace
        state={{ from: location }}
      />
    );
  }

  const role = getRoleFromToken(accessToken);

  if (!role) {
    return <Navigate to="/unauthorized" replace />;
  }

  // Any authenticated user can access
  if (!allowedRoles) {
    return <Outlet />;
  }

  // Admin can access everything
  if (role === "Admin") {
    return <Outlet />;
  }

  // Check if user's role is allowed
  if (allowedRoles.includes(role)) {
    return <Outlet />;
  }

  // User is logged in but doesn't have permission
  return <Navigate to="/unauthorized" replace />;
};