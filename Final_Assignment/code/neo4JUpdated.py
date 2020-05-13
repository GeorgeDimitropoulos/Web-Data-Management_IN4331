from neo4jrestclient.client import GraphDatabase
from neo4jrestclient import client as cl
import pandas as pd
import numpy as np


db = GraphDatabase("http://localhost:7474", username="neo4j", password="1234")

datafr=pd.read_csv('C:\\Users\\pc1\\Desktop\\NeoJ\\movies.csv')
sorteddf=datafr.sort_values("dname")
sorteddf.dropna(axis=1)
# Create some nodes with labels

directorList=[]
actorList=[]
director = db.labels.create("director")
movies = db.labels.create("movie")
actors=db.labels.create("actor")

for index,row in sorteddf.iterrows():
    if row["dname"]!='nan' and row["movie_title"]!='nan':
        if row["dname"] not in directorList:
            u1 = db.nodes.create(name=row["dname"])
            director.add(u1)

            movie=db.nodes.create(name=row["movie_title"])
            movies.add(movie)

            u1.relationships.create("directed",movie)
            directorList.append(row["dname"])
            actorsToCheck=[row["actor_1_name"],row["actor_2_name"],row["actor_3_name"] ]
            for actr in actorsToCheck:
                if(actr not in actorList):
                    actor = db.nodes.create(name=actr)
                    actors.add(actor)
                    movie.relationships.create("played",actor)
                    actorList.append(actr)
                else:
                    actorq = 'MATCH (actor:actors) WHERE actor.name= ' +actr+ ' RETURN actor'
                    resultedActor = db.query(actorq, returns=(cl.Node))
                    movie.relationships.create("played",resultedActor)

        else:
            movie = db.nodes.create(name=row["movie_title"])
            movies.add(movie)

            u1.relationships.create("directed", movie)

            actorsToCheck=[row["actor_1_name"],row["actor_2_name"],row["actor_3_name"] ]
            for actr in actorsToCheck:
                if(actr not in actorList):
                    actor = db.nodes.create(name=actr)
                    actors.add(actor)
                    movie.relationships.create("played",actor)
                    actorList.append(actr)
                else:
                    actorq = 'MATCH (actor:actors) WHERE actor.name= ' +actr+ ' RETURN actor'
                    resultedActor = db.query(actorq, returns=(cl.Node))
                    movie.relationships.create("played",resultedActor)



