import { Box, Button, Typography } from "@mui/material";
import { useNavigate } from "react-router-dom";

const LoginPage = () => {
  const navigate = useNavigate();

  const handleDummyLogin = () => {
    navigate("/employees");
  };

  return (
    <Box sx={{ p: 4 }}>
      <Typography variant="h4" gutterBottom>
        Login
      </Typography>

      <Typography>
        This is the login page.
      </Typography>

      <Button variant="contained" onClick={handleDummyLogin}>
        Dummy Login
      </Button>
    </Box>
  );
};

export default LoginPage;