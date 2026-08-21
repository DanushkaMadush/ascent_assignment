import { Box, Typography } from "@mui/material";

const UnauthorizedPage = () => {


  return (
    <Box sx={{ p: 4 }}>
      <Typography variant="h4" gutterBottom>
        Unauthorized
      </Typography>

      <Typography>
        You have no access
      </Typography>
    </Box>
  );
};

export default UnauthorizedPage;