import { Box, Button, Card, CardContent, Stack, Typography } from "@mui/material";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../auth/AuthContext";


type UserRole = "Admin" | "Manager" | "Employee";

const HomePage = () => {
  const navigate = useNavigate();
  const { user, signOut } = useAuth();
  
  const handleSignOut = () => {
    signOut();
    navigate("/login", { replace: true });
  };

  if (!user) {
    return (
      <Box
        sx={{
          minHeight: "100vh",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          p: 2,
        }}
      >
        <Typography>Unable to load user information.</Typography>
      </Box>
    );
  }

  const fullName = `${user.firstName} ${user.lastName}`.trim();

  const canAccess = (roles: UserRole[]) => roles.includes(user.role as UserRole);

  return (
    <Box
      sx={{
        minHeight: "100vh",
        display: "flex",
        justifyContent: "center",
        alignItems: "flex-start",
        bgcolor: "#f5f5f5",
        p: 3,
      }}
    >
      <Card
        sx={{
          width: "100%",
          maxWidth: 500,
          mt: 8,
          borderRadius: 2,
        }}
      >
        <CardContent sx={{ p: 4 }}>
          <Stack spacing={3}>
            <Box>
              <Typography variant="h5" sx={{ fontWeight: 600 }}>
                Hi, {fullName}
              </Typography>

              <Typography
                variant="body1"
                color="text.secondary"
                sx={{ mt: 1 }}
              >
                Welcome back!
              </Typography>
            </Box>

            <Stack spacing={1.5}>
              {canAccess(["Admin"]) && (
                <Button
                  variant="contained"
                  fullWidth
                  onClick={() => navigate("/employees")}
                >
                  Employee Management
                </Button>
              )}

              {canAccess(["Admin"]) && (
                <Button
                  variant="contained"
                  fullWidth
                  onClick={() => navigate("/leaves")}
                >
                  Leave Management
                </Button>
              )}

              {canAccess(["Admin", "Manager"]) && (
                <Button
                  variant="contained"
                  fullWidth
                  onClick={() => navigate("/attendance")}
                >
                  Attendance Management
                </Button>
              )}

              {canAccess(["Admin"]) && (
                <Button
                  variant="contained"
                  fullWidth
                  onClick={() => navigate("/departments")}
                >
                  Department Management
                </Button>
              )}

              {canAccess(["Admin", "Manager", "Employee"]) && (
                <Button
                  variant="contained"
                  fullWidth
                  onClick={() => navigate("/profile")}
                >
                  My Profile
                </Button>
              )}

              {canAccess(["Admin", "Manager", "Employee"]) && (
                <Button
                  variant="outlined"
                  fullWidth
                  onClick={ handleSignOut }
                >
                  Sign Out
                </Button>
              )}
            </Stack>
          </Stack>
        </CardContent>
      </Card>
    </Box>
  );
};

export default HomePage;