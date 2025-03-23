import React, { useEffect, useState } from "react";
import { BrowserRouter as Router, Route, Routes, Link, useLocation, useNavigate } from "react-router-dom";
import { Button, Container, VStack, HStack, Box, Grid, useBreakpointValue } from "@chakra-ui/react";
import {
  DrawerRoot,
  DrawerBackdrop,
  DrawerTrigger,
  DrawerContent,
  DrawerHeader,
  DrawerTitle,
  DrawerBody,
  DrawerCloseTrigger,
} from "@/components/ui/drawer";
import CreateAccount from "./myComponents/CreateAccount";
import GetUsers from "./myComponents/user/GetUsers";
import GetUser from "./myComponents/user/GetUser";
import Profile from "./myComponents/user/Profile";
import Login from "./myComponents/Login";
import ChangePassword from "./myComponents/user/ChangePassword"; 
import { getUserRole } from "./services/InfoJwt/getUserRole";
import { getUserEmail } from "./services/InfoJwt/getUserEmail";
import { logout } from "./services/logout";
import axios from "axios";
import CreateAdvertisement from "./myComponents/advertisement/CreateAdvertisement";
import GetAdvertisements from "./myComponents/advertisement/GetAdvertisements";
import GetMyAdvertisements from "./myComponents/advertisement/GetMyAdvertisements";
import GetAllAdvertisements from "./myComponents/advertisement/GetAllAdvertisements";
import GetAdvertisementDetails from "./myComponents/advertisement/AdvertisementDetails";

const Layout = () => {
  const buttonSize = useBreakpointValue({ base: "xs", md: "xs", lg: "sm" });
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [role, setRole] = useState(null);
  const [email, setEmail] = useState(null);
  const navigate = useNavigate();
  const location = useLocation(); // Получаем текущий маршрут

  const logoutSubmit = async () => {
    const response = await logout();
    if (!response.success) {
      // Обработка ошибок (при необходимости)
    } else {
      localStorage.removeItem("authToken");
      setIsAuthenticated(false);
      setRole(null);
      setEmail(null);
      navigate("/Advertisements/1"); // Перенаправляем на главную
    }
  };
  axios.interceptors.response.use(
    response => response,
    error => {
      if (error.response && error.response.status === 401 &&localStorage.getItem("authToken")!=null) {
        // Удаляем токен из localStorage или другого хранилища
        localStorage.removeItem('authToken');
        window.location.href = '/login';
      }
      return Promise.reject(error);
    }
  );
  useEffect(() => {
    const checkAuth = () => {
      const token = localStorage.getItem("authToken");
      console.log(token);
      setIsAuthenticated(!!token);
      if (token) {
        setRole(getUserRole());
        setEmail(getUserEmail());
      } else {
        setRole(null);
        setEmail(null);
      }
    };

    checkAuth();
  }, [location]); // Перепроверка при изменении маршрута

  return (
    <Box as="nav" position="fixed" top="0" w="100%" bg="white" shadow="xs" p={0.2} zIndex="101">
      <Grid templateColumns="minmax(4rem, 1fr) auto minmax(4rem, 1fr)" alignItems="center">
        {/* Левое меню (Drawer) */}
        <HStack p={4} flex="1">
          <DrawerRoot placement="left">
            <DrawerTrigger asChild>
              <Button variant="outline" size={buttonSize}>☰ Меню</Button>
            </DrawerTrigger>
            <DrawerBackdrop />
            <DrawerContent width={{ base: "75%", md: "16rem" }} height="100vh" boxShadow="lg" zIndex="101">
              <DrawerHeader>
                <DrawerTitle>Навигация</DrawerTitle>
              </DrawerHeader>
              <DrawerBody>
                <VStack align="start" spacing={4}>
                <Link to="/Advertisements/1">
                    <Button variant="ghost" w="full" size={buttonSize}>Главная</Button>
                  </Link>
                  {!isAuthenticated ? (
                    <>
                      <Link to="/create-account">
                        <Button variant="ghost" w="full" size={buttonSize}>Создать аккаунт</Button>
                      </Link>
                      <Link to="/login">
                        <Button variant="solid" colorScheme="blue" w="full" size={buttonSize}>Вход в аккаунт</Button>
                      </Link>
                    </>
                  ) : (
                    <>
                      <Button variant="solid" onClick={logoutSubmit} width="full" mt={4}>Выйти</Button>
                      {role === "Admin" && (
                        <Link to="/users">
                          <Button variant="ghost" w="full" size={buttonSize}>Список пользователей</Button>
                        </Link>
                      )}
                    </>
                  )}
                </VStack>
              </DrawerBody>
              <DrawerCloseTrigger />
            </DrawerContent>
          </DrawerRoot>
        </HStack>

        {/* Центральное меню */}
        <HStack justify="center" display={{ base: "none", md: "flex" }}>
        <Link to="/Advertisements/1">
            <Button variant="solid" size={buttonSize}>Главная</Button>
          </Link>
          <Link to="/users">
            <Button variant="solid" size={buttonSize}>Список пользователей</Button>
          </Link>
          <Link to="/create-account">
            <Button variant="solid" size={buttonSize}>Создать аккаунт</Button>
          </Link>
        </HStack>

        {/* Правое меню */}
        <HStack position="absolute" right="0" p={4} zIndex="101">
          <DrawerRoot>
            <DrawerTrigger asChild>
              <Button variant="outline" size={buttonSize}>☰ Меню (Правое)</Button>
            </DrawerTrigger>
            <DrawerBackdrop />
            <DrawerContent width={{ base: "75%", md: "16rem" }} height="100vh" boxShadow="lg" zIndex="101">
              <DrawerHeader>
                <DrawerTitle>Навигация</DrawerTitle>
              </DrawerHeader>
              <DrawerBody>
                <VStack align="start" spacing={4}>
                <Link to="/Advertisements/1">
                    <Button variant="ghost" w="full" size={buttonSize}>Главная</Button>
                  </Link>
                  {!isAuthenticated ? (
                    <>
                      <Link to="/create-account">
                        <Button variant="ghost" w="full" size={buttonSize}>Создать аккаунт</Button>
                      </Link>
                      <Link to="/login">
                        <Button variant="solid" colorScheme="blue" w="full" size={buttonSize}>Вход в аккаунт</Button>
                      </Link>
                    </>
                  ) : (
                    <>
                       <Link to="/me">
                        <Button variant="ghost" w="full" size={buttonSize}>Профиль</Button>
                      </Link>
                      <Link to="/MyAdvertisements/1">
                        <Button variant="ghost" w="full" size={buttonSize}>Ваши объявления</Button>
                      </Link>
                      <Link to="/change-password">
                        <Button variant="ghost" w="full" size={buttonSize}>Смена пароля</Button>
                      </Link>
                      <Link to="/create-advertisement">
                        <Button variant="ghost" w="full" size={buttonSize}>Создать объявление</Button>
                      </Link>
                      {role === "Admin" && (
                        <>
                        <Link to="/users">
                          <Button variant="ghost" w="full" size={buttonSize}>Список пользователей</Button>
                        </Link>
                        <Link to="/AllAdvertisements/1">
                          <Button variant="ghost" w="full" size={buttonSize}>Все объявления</Button>
                        </Link>
                        </>
                      )}
                    </>
                  )}
                </VStack>
              </DrawerBody>
              <DrawerCloseTrigger />
            </DrawerContent>
          </DrawerRoot>
        </HStack>
      </Grid>
    </Box>
  );
};

const Home = () => (
  <VStack spacing={6} p={6}>
    <h1 className="text-2xl font-bold">Добро пожаловать!</h1>
    <p className="text-gray-600">Выберите действие через меню ☰</p>
  </VStack>
);

const App = () => {
  return (
    <Router>
      <Layout />
      <Grid position="fixed top-0 left-0" templateRows="auto 1fr" minH="100vh">
        <Container
          position="absolute"
          maxW={{ base: "100%", md: "container.md", sm: "container.sm", lg: "container.lg", xl: "container.xl" }}
          p={4}
          mt="70px"
          overflowY="auto"
          flex="1"
        >
          <Routes>
          <Route path="/Advertisements/:page" element={<GetAdvertisements />} />
          <Route path="/MyAdvertisements/:page" element={<GetMyAdvertisements />} />
          <Route path="/AllAdvertisements/:page" element={<GetAllAdvertisements />} />
          <Route path="/advertisement/:id" element={<GetAdvertisementDetails />} />
          
            <Route path="/users" element={<GetUsers />} />
            <Route path="/me" element={<Profile />} />
            <Route path="/user/:id" element={<GetUser />} />
            <Route path="/create-account" element={<CreateAccount />} />
            <Route path="/login" element={<Login />} />
            <Route path="/change-password" element={<ChangePassword />} /> {/* Добавляем маршрут для смены пароля */}
            <Route path="/create-advertisement" element={<CreateAdvertisement />} /> {/* Добавляем маршрут для смены пароля */}
          </Routes>
        </Container>
      </Grid>
    </Router>
  );
};

export default App;