import React, { useEffect, useState } from "react";
import { BrowserRouter as Router, Route, Routes, Link, useLocation, useNavigate } from "react-router-dom";
import { Button, Container, VStack, HStack, Box, Grid, useBreakpointValue, Drawer, Accordion, Text, IconButton  } from "@chakra-ui/react";
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
import { CgProfile } from "react-icons/cg";
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
import CreateRentalRequest from "./myComponents/rentalRequests/CreateRentalRequest";
import GetAdvertisements from "./myComponents/advertisement/GetAdvertisements";
import GetMyAdvertisements from "./myComponents/advertisement/GetMyAdvertisements";
import GetAllAdvertisements from "./myComponents/advertisement/GetAllAdvertisements";
import GetAdvertisementDetails from "./myComponents/advertisement/AdvertisementDetails";
import GetRentalRequests from "./myComponents/rentalRequests/GetRentalRequests";
import GetRentalRequest from "./myComponents/rentalRequests/GetRentalRequest";
import GetMyRentalRequests from "./myComponents/rentalRequests/getMyRentalRequests";
import GetMySentRentalRequests from "./myComponents/rentalRequests/getMySentRentalRequests";
import CreateReview from "./myComponents/review/CreateReview";
import { useColorMode, useColorModeValue } from "@/components/ui/color-mode"

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

  const color = `${COLOR}`;
  const bg = `${BG}`;

   // Состояние для отслеживания открытого аккордеона
   const [expandedItems, setExpandedItems] = useState({});

   const handleAccordionChange = (value) => {
     setExpandedItems((prevState) => ({
       ...prevState,
       [value]: !prevState[value], // Переключение состояния для каждого элемента
     }));
   };

   
 
   return (
    <Box as="nav" position="fixed" top="0" w="100%" p={0.2} zIndex="101" color={color}>
      <Grid templateColumns="minmax(4rem, 1fr) auto minmax(4rem, 1fr)" alignItems="center">
        <HStack p={4} wrap="wrap">
          <Drawer.Root key="left" placement="left">
            <Drawer.Trigger asChild>
              <Button variant="outline" color={color} size={buttonSize}>☰</Button>
            </Drawer.Trigger>
            <Drawer.Positioner>
              <Drawer.Content width={{ base: "75%", md: "16rem" }} height="100vh" boxShadow="lg" zIndex="101" color={color} bg={bg}>
                <Drawer.CloseTrigger />
                <Drawer.Header>
                  <Drawer.Title>Меню</Drawer.Title>
                </Drawer.Header>
                <Drawer.Body>
                  <Accordion.Root>
                    {!isAuthenticated ? (
                      <Accordion.Item>
                        <Accordion.ItemTrigger onClick={() => handleAccordionChange("guest")}>
                          <Box flex="1" textAlign="left">Гость</Box>
                          <Accordion.ItemIndicator />
                        </Accordion.ItemTrigger>
                        {expandedItems["guest"] && (
                          <Accordion.ItemContent>
                            <Accordion.ItemBody>
                              <VStack spacing={2} align="stretch">
                                <Button bg="green" color="white" onClick={() => navigate(`/create-account`)}>Создать аккаунт</Button>
                                <Button bg="blue" color="white" onClick={() => navigate(`/login`)}>Войти</Button>
                              </VStack>
                            </Accordion.ItemBody>
                          </Accordion.ItemContent>
                        )}
                      </Accordion.Item>
                    ) : (
                      <>
                  <Text fontSize="sm" color={color} mt={2}>
                    {email}
                  </Text>
                        <Accordion.Item>
                          <Accordion.ItemTrigger onClick={() => handleAccordionChange("user")}>
                            <Box flex="1" textAlign="left">Пользователь</Box>
                            <Accordion.ItemIndicator />
                          </Accordion.ItemTrigger>
                          {expandedItems["user"] && (
                            <Accordion.ItemContent>
                              <Accordion.ItemBody>
                                <VStack spacing={2} align="stretch">
                                  <Link to="/me"><Button variant="link">Профиль</Button></Link>
                                  <Link to="/change-password"><Button variant="link">Смена пароля</Button></Link>
                                  <Button onClick={logoutSubmit} bg="red" color="white">Выйти</Button>
                                </VStack>
                              </Accordion.ItemBody>
                            </Accordion.ItemContent>
                          )}
                        </Accordion.Item>
  
                        <Accordion.Item>
                          <Accordion.ItemTrigger onClick={() => handleAccordionChange("ads")}>
                            <Box flex="1" textAlign="left">Объявления</Box>
                            <Accordion.ItemIndicator />
                          </Accordion.ItemTrigger>
                          {expandedItems["ads"] && (
                            <Accordion.ItemContent>
                              <Accordion.ItemBody>
                                <VStack spacing={2} align="stretch">
                                  <Link to="/MyAdvertisements/1"><Button variant="link">Мои объявления</Button></Link>
                                  <Link to="/myRentalRequests/1"><Button variant="link">На мои объявления</Button></Link>
                                  <Link to="/create-advertisement"><Button variant="link">Создать новое объявление</Button></Link>
                                </VStack>
                              </Accordion.ItemBody>
                            </Accordion.ItemContent>
                          )}
                        </Accordion.Item>
  
                        <Accordion.Item>
                          <Accordion.ItemTrigger onClick={() => handleAccordionChange("rentalRequests")}>
                            <Box flex="1" textAlign="left">Запросы на аренду</Box>
                            <Accordion.ItemIndicator />
                          </Accordion.ItemTrigger>
                          {expandedItems["rentalRequests"] && (
                            <Accordion.ItemContent>
                              <Accordion.ItemBody>
                                <VStack spacing={2} align="stretch">
                                  <Link to="/mySentRentalRequests/1"><Button variant="link">Мои запросы</Button></Link>
                                  <Link to="/myRentalRequests/1"><Button variant="link">На мои объявления</Button></Link>
                                </VStack>
                              </Accordion.ItemBody>
                            </Accordion.ItemContent>
                          )}
                        </Accordion.Item>
                        {role === "Admin" && (
                          <Accordion.Item>
                            <Accordion.ItemTrigger onClick={() => handleAccordionChange("admin")}>
                              <Box flex="1" textAlign="left">Администратор</Box>
                              <Accordion.ItemIndicator />
                            </Accordion.ItemTrigger>
                            {expandedItems["admin"] && (
                              <Accordion.ItemContent>
                                <Accordion.ItemBody>
                                  <VStack spacing={2} align="stretch">
                                    <Link to="/users"><Button >Пользователи</Button></Link>
                                    <Link to="/AllAdvertisements/1"><Button>Все объявления</Button></Link>
                                  </VStack>
                                </Accordion.ItemBody>
                              </Accordion.ItemContent>
                            )}
                          </Accordion.Item>
                        )}
                      </>
                    )}
                  </Accordion.Root>
                </Drawer.Body>
                <Drawer.Footer />
              </Drawer.Content>
            </Drawer.Positioner>
          </Drawer.Root>
        </HStack>
        <HStack justify="flex-end" p={4}>
    {isAuthenticated && (

        <IconButton  aria-label="Search database"
         variant="link" size = "2xl" position="fixed" top="0" right="0" rounded="full" color={color} onClick={() => navigate("/me")} >
        <CgProfile size={40}/>
        </IconButton>
    )}
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
      <Grid position="fixed top-0 left-0" templateRows="auto 1fr"   w="100vw" 
    h="150vh"     backgroundImage="url('/public/bgsity.jpg')" // backgroundImage="url('/public/screen-2.jpg')"
    backgroundSize="cover"
    backgroundPosition="center"
    backgroundRepeat="no-repeat"
    >
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
          <Route path="/createRentalRequest/:idNeedAdvertisement" element={<CreateRentalRequest />} />
          <Route path="/createReview/:idNeedRentalRequest" element={<CreateReview />} />

          <Route path="/rentalRequests/:idNeedAdvertisement/:page" element={<GetRentalRequests />} />
          <Route path="/myRentalRequests/:page" element={<GetMyRentalRequests />} />
          <Route path="/mySentRentalRequests/:page" element={<GetMySentRentalRequests />} />

          
            <Route path="/users" element={<GetUsers />} />
            <Route path="/me" element={<Profile />} />
            <Route path="/user/:id" element={<GetUser />} />
            <Route path="/rentalRequest/:id" element={<GetRentalRequest />} />
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